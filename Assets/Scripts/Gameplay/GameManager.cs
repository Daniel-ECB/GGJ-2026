using GGJ2026.Audio;
using GGJ2026.Core.Utils;
using GGJ2026.Troupe;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        [Header("Game Over")]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private CanvasGroup _gameOverCanvasGroup;
        [SerializeField] private float _gameOverFadeDuration = 0.25f;
        [SerializeField] private float _gameOverHiddenScale = 0.95f;
        [SerializeField] private TMP_Text _resultTitleText;
        [SerializeField] private Image _resultTitleImage;
        [SerializeField] private Sprite _victoryTitleSprite;
        [SerializeField] private Sprite _defeatTitleSprite;
        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private TMP_Text _finalStarsText;
        [SerializeField] private Image _starsImage;
        [SerializeField] private Sprite _stars1Sprite;
        [SerializeField] private Sprite _stars2Sprite;
        [SerializeField] private Sprite _stars3Sprite;
        [SerializeField] private Sprite _stars4Sprite;
        [SerializeField] private Sprite _stars5Sprite;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private AudioSource _crashSfx;
        [SerializeField] private string _menuSceneName = "";
        [SerializeField] private string _victoryTitle = "Victoria!";
        [SerializeField] private string _defeatTitle = "Derrota!";

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
        private Coroutine _gameOverFadeRoutine;

        public event System.Action OnPlayerMistake;
        public event System.Action<MaskColors, HitOutcome> OnBlockResolved;

        public float CurrentScore => _currentScore;

        [Header("Spawner Reference")]
        [SerializeField] private Spawner spawner;

        
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

            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(false);

                if (_gameOverCanvasGroup == null)
                    _gameOverCanvasGroup = _gameOverPanel.GetComponent<CanvasGroup>();
                if (_gameOverCanvasGroup == null)
                    _gameOverCanvasGroup = _gameOverPanel.AddComponent<CanvasGroup>();

                _gameOverCanvasGroup.alpha = 0f;
                _gameOverCanvasGroup.interactable = false;
                _gameOverCanvasGroup.blocksRaycasts = false;
            }

            if (_retryButton != null)
                _retryButton.onClick.AddListener(Retry);

            if (_quitButton != null)
                _quitButton.onClick.AddListener(QuitToMenu);
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
                bool victory = remainingBlocks == 0 && _lives > 0;
                EndGame(victory);
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
            if (spawner != null)
                Debug.Log($"Bloques resueltos: {_blocksResolved}/{spawner.NumberOfBlocks}");
        }

        public void EndGame(bool victory)
        {
            if (_gameEnded)
                return;

            _gameEnded = true;

            int totalBlocks = _blocksHit + _blocksFailed;
            float accuracy = totalBlocks > 0 ? ((float)_blocksHit / totalBlocks) * 100f : 0f;

            int stars;
            if (accuracy >= 90f) stars = 5;
            else if (accuracy >= 75f) stars = 4;
            else if (accuracy >= 50f) stars = 3;
            else if (accuracy >= 25f) stars = 2;
            else stars = 1;

            Debug.Log($"Game Over, Final Score: {_currentScore}, Precision: {accuracy}%, Stars: {stars}");

            if (_troupeMovement != null)
                _troupeMovement.StopMovement();

            if (_musicLayers != null)
                _musicLayers.StopAll();

            if (_crashSfx != null)
                _crashSfx.Play();

            if (_resultTitleImage != null)
            {
                Sprite titleSprite = victory ? _victoryTitleSprite : _defeatTitleSprite;
                if (titleSprite != null)
                {
                    _resultTitleImage.sprite = titleSprite;
                    _resultTitleImage.enabled = true;
                }
            }

            if (_resultTitleText != null)
                _resultTitleText.text = victory ? _victoryTitle : _defeatTitle;

            if (_finalScoreText != null)
                _finalScoreText.text = $"Score: {Mathf.RoundToInt(_currentScore)}";

            if (_finalStarsText != null)
                _finalStarsText.text = $"Stars: {stars}";

            if (_starsImage != null)
            {
                Sprite starsSprite = stars switch
                {
                    1 => _stars1Sprite,
                    2 => _stars2Sprite,
                    3 => _stars3Sprite,
                    4 => _stars4Sprite,
                    5 => _stars5Sprite,
                    _ => null
                };

                if (starsSprite != null)
                {
                    _starsImage.sprite = starsSprite;
                    _starsImage.enabled = true;
                }
            }

            if (_gameOverPanel != null)
                StartGameOverFade();

            Time.timeScale = 0f;
        }

        private void StartGameOverFade()
        {
            if (_gameOverCanvasGroup == null)
            {
                if (_gameOverPanel != null)
                    _gameOverPanel.SetActive(true);
                return;
            }

            if (_gameOverFadeRoutine != null)
                StopCoroutine(_gameOverFadeRoutine);

            _gameOverFadeRoutine = StartCoroutine(GameOverFadeRoutine());
        }

        private IEnumerator GameOverFadeRoutine()
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(true);

            _gameOverCanvasGroup.interactable = false;
            _gameOverCanvasGroup.blocksRaycasts = false;

            float startAlpha = _gameOverCanvasGroup.alpha;
            float endAlpha = 1f;
            float t = 0f;

            RectTransform rt = _gameOverCanvasGroup.GetComponent<RectTransform>();
            Vector3 startScale = rt != null ? rt.localScale : Vector3.one;
            Vector3 endScale = Vector3.one;
            Vector3 hiddenScale = Vector3.one * _gameOverHiddenScale;

            // Ensure we start from hidden scale if currently hidden
            if (rt != null && startAlpha <= 0f)
                rt.localScale = hiddenScale;

            while (t < _gameOverFadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = _gameOverFadeDuration <= 0f ? 1f : Mathf.Clamp01(t / _gameOverFadeDuration);
                _gameOverCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, k);
                if (rt != null)
                    rt.localScale = Vector3.Lerp(hiddenScale, endScale, k);
                yield return null;
            }

            _gameOverCanvasGroup.alpha = 1f;
            if (rt != null)
                rt.localScale = endScale;

            _gameOverCanvasGroup.interactable = true;
            _gameOverCanvasGroup.blocksRaycasts = true;
        }

        private void Retry()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void QuitToMenu()
        {
            Time.timeScale = 1f;
            if (!string.IsNullOrEmpty(_menuSceneName))
                SceneManager.LoadScene(_menuSceneName);
            else
                Application.Quit();
        }
    }
}
