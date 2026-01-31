using GGJ2026.Core.Utils;
using System.Collections;
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
        private int _blocksResolved = 0;
        private bool _gameEnded = false;
        private float _playerErrorTimer = 0.0f;

        public event System.Action OnPlayerMistake;
        public event System.Action<MaskColors, HitOutcome> OnBlockResolved;

        public float CurrentScore => _currentScore;

        [Header("Spawner Reference")]
        [SerializeField] private Spawner spawner;

        private void Update()
        {
            if (_gameEnded) return;

            
            if (spawner != null && spawner.FinishedSpawning && _blocksResolved >= spawner.NumberOfBlocks)
            {
                EndGame();
                Time.timeScale = 0f;
                _gameEnded = true;
                Debug.Log("Fin del juego!");
            }

            
            if (_lives <= 0)
            {
                EndGame();
                Time.timeScale = 0f;
                _gameEnded = true;
                Debug.Log("Fin del juego por vidas!");
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

        public void NotifyBlockResolved(MaskColors color, HitOutcome outcome)
        {
            OnBlockResolved?.Invoke(color, outcome);
        }

        public void BlockResolved()
        {
            _blocksResolved++;
            Debug.Log($"Bloques resueltos: {_blocksResolved}/{spawner.NumberOfBlocks}");
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