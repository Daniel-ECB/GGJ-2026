using UnityEngine;
using UnityInput = UnityEngine.Input;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GGJ2026.Audio;
using GGJ2026.Troupe;

namespace GGJ2026.UI
{
    public sealed class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private KeyCode _toggleKey = KeyCode.Escape;

        [Header("Audio/Movement")]
        [SerializeField] private MusicLayerController _musicLayers;
        [SerializeField] private TroupeMovement _troupeMovement;

        [Header("Transition")]
        [SerializeField] private CanvasGroup _pauseCanvasGroup;
        [SerializeField] private float _fadeDuration = 0.2f;
        [SerializeField] private float _hiddenScale = 0.95f;

        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private string _menuSceneName = "";

        private bool _paused;
        private Coroutine _fadeRoutine;

        private void Awake()
        {
            if (_musicLayers == null)
                _musicLayers = FindFirstObjectByType<MusicLayerController>();
            if (_troupeMovement == null)
                _troupeMovement = FindFirstObjectByType<TroupeMovement>();

            if (_pausePanel != null)
            {
                _pausePanel.SetActive(false);

                if (_pauseCanvasGroup == null)
                    _pauseCanvasGroup = _pausePanel.GetComponent<CanvasGroup>();
                if (_pauseCanvasGroup == null)
                    _pauseCanvasGroup = _pausePanel.AddComponent<CanvasGroup>();

                _pauseCanvasGroup.alpha = 0f;
                _pauseCanvasGroup.interactable = false;
                _pauseCanvasGroup.blocksRaycasts = false;
            }

            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(Resume);
            if (_retryButton != null)
                _retryButton.onClick.AddListener(Retry);
            if (_quitButton != null)
                _quitButton.onClick.AddListener(QuitToMenu);
        }

        private void Update()
        {
            if (UnityInput.GetKeyDown(_toggleKey))
            {
                if (_gameOverPanel != null && _gameOverPanel.activeInHierarchy)
                    return;

                if (_paused)
                    Resume();
                else
                    Pause();
            }
        }

        public void Pause()
        {
            if (_paused) return;
            _paused = true;

            if (_pausePanel != null)
                _pausePanel.SetActive(true);

            _musicLayers?.PauseAll();
            _troupeMovement?.PauseMovement();

            Time.timeScale = 0f;

            StartFade(true);
        }

        public void Resume()
        {
            if (!_paused) return;
            _paused = false;

            Time.timeScale = 1f;

            _musicLayers?.UnpauseAll();
            _troupeMovement?.ResumeMovement();

            StartFade(false);
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

        private void StartFade(bool show)
        {
            if (_pauseCanvasGroup == null)
            {
                if (_pausePanel != null)
                    _pausePanel.SetActive(show);
                return;
            }

            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(FadeRoutine(show));
        }

        private System.Collections.IEnumerator FadeRoutine(bool show)
        {
            if (_pausePanel != null && show)
                _pausePanel.SetActive(true);

            _pauseCanvasGroup.interactable = false;
            _pauseCanvasGroup.blocksRaycasts = false;

            float startAlpha = _pauseCanvasGroup.alpha;
            float endAlpha = show ? 1f : 0f;
            float t = 0f;

            RectTransform rt = _pauseCanvasGroup.GetComponent<RectTransform>();
            Vector3 startScale = rt != null ? rt.localScale : Vector3.one;
            Vector3 endScale = show ? Vector3.one : Vector3.one * _hiddenScale;

            while (t < _fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = _fadeDuration <= 0f ? 1f : Mathf.Clamp01(t / _fadeDuration);
                _pauseCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, k);
                if (rt != null)
                    rt.localScale = Vector3.Lerp(startScale, endScale, k);
                yield return null;
            }

            _pauseCanvasGroup.alpha = endAlpha;
            if (rt != null)
                rt.localScale = endScale;

            if (show)
            {
                _pauseCanvasGroup.interactable = true;
                _pauseCanvasGroup.blocksRaycasts = true;
            }
            else
            {
                if (_pausePanel != null)
                    _pausePanel.SetActive(false);
            }
        }
    }
}
