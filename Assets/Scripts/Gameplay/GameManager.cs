using GGJ2026.Core.Utils;
using System.Collections;
using System.Linq;
using UnityEngine;
using GGJ2026.Audio;
using GGJ2026.Troupe;

namespace GGJ2026.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class GameManager : Singleton<GameManager>
    {
        [Header("Start Sequence")]
        [SerializeField] private bool _autoStartRun = true;
        [SerializeField] private float _startDelaySec = 1.0f;
        [SerializeField] private MusicLayerController _musicLayers;
        [SerializeField] private TroupeMovement _troupeMovement;

        [SerializeField] private float _playerErrorCooldown = 1.25f;

        private float _currentScore = 0.0f;
        private float _scoreMultiplier = 1.0f;
        private int _strikes = 0;
        private int _lives = 5;
        private int _blocksHit = 0;
        private int _blocksFailed = 0;
        private bool _gameEnded = false;
        private float _playerErrorTimer = 0.0f;

        public event System.Action OnPlayerMistake;

        public float CurrentScore => _currentScore;

        protected override void Awake()
        {
            base.Awake();

            if (_musicLayers == null)
                _musicLayers = FindFirstObjectByType<MusicLayerController>();
            if (_troupeMovement == null)
                _troupeMovement = FindFirstObjectByType<TroupeMovement>();

            if (_musicLayers != null)
                _musicLayers.DisableAutoStart();
            if (_troupeMovement != null)
                _troupeMovement.DisableAutoStart();
        }

        private void Start()
        {
            if (_autoStartRun)
                StartCoroutine(StartRunSequence());
        }

        private IEnumerator StartRunSequence()
        {
            double dspStart = AudioSettings.dspTime + _startDelaySec;

            if (_musicLayers != null)
                _musicLayers.StartAtDspTime(dspStart);

            if (_startDelaySec > 0f)
                yield return new WaitForSecondsRealtime(_startDelaySec);

            if (_troupeMovement != null)
                _troupeMovement.BeginAtDspTime(dspStart);
        }

        private void Update()
        {
            if (_gameEnded) return;

            int remainingBlocks = Object.FindObjectsByType<CarnivalBlock>(FindObjectsSortMode.None)
                                         .Count(b => b != null && b.gameObject.activeInHierarchy);

            if (remainingBlocks == 0 || _lives <= 0)
            {
                EndGame();
                Time.timeScale = 0f;
                _gameEnded = true;
                Debug.Log("Fin del juego!");
            }

            if (_playerErrorTimer < _playerErrorCooldown)
                _playerErrorTimer += Time.deltaTime;
        }

        public void ApprovedBlock()
        {
            _strikes = 0;
            _scoreMultiplier += 0.01f;
            _currentScore += 100f * _scoreMultiplier;
            _blocksHit++;

            Debug.Log($"Score added! The current score is {_currentScore}");
        }

        public void FailedBlock()
        {
            if (_playerErrorTimer < _playerErrorCooldown)
                return;

            _strikes++;
            _scoreMultiplier = 1.0f;
            _blocksFailed++;

            Debug.Log("Score deducted!");

            if (_strikes >= 2)
            {
                _playerErrorTimer = 0.0f;
                _lives--;
                _strikes = 0;
                OnPlayerMistake?.Invoke();
                Debug.Log($"Vidas restantes: {_lives}");
            }
            else
            {
                Debug.Log("Strike");
            }
        }

        public void EndGame()
        {
            int totalBlocks = _blocksHit + _blocksFailed;
            float accuracy = totalBlocks > 0 ? ((float)_blocksHit / totalBlocks) * 100f : 0f;

            int stars;
            if (accuracy >= 90f) stars = 5;
            else if (accuracy >= 75f) stars = 4;
            else if (accuracy >= 50f) stars = 3;
            else if (accuracy >= 25f) stars = 2;
            else stars = 1;

            Debug.Log($"Game Over, Final Score: {_currentScore}, Precision: {accuracy}%, Stars: {stars}");
        }
    }
}
