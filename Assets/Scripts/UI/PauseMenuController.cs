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

        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private string _menuSceneName = "";

        private bool _paused;

        private void Awake()
        {
            if (_musicLayers == null)
                _musicLayers = FindFirstObjectByType<MusicLayerController>();
            if (_troupeMovement == null)
                _troupeMovement = FindFirstObjectByType<TroupeMovement>();

            if (_pausePanel != null)
                _pausePanel.SetActive(false);

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
        }

        public void Resume()
        {
            if (!_paused) return;
            _paused = false;

            if (_pausePanel != null)
                _pausePanel.SetActive(false);

            Time.timeScale = 1f;

            _musicLayers?.UnpauseAll();
            _troupeMovement?.ResumeMovement();
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
