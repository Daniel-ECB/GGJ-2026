using GGJ2026.Core.Utils;
using System.Linq;
using UnityEngine;

namespace GGJ2026.Gameplay
{
    [DisallowMultipleComponent]
    public class GameManager : Singleton<GameManager>
    {
        private float _currentScore = 0.0f;
        private float _scoreMultiplier = 1.0f;
        private int _strikes = 0;
        private int _lives = 5;
        private int _blocksHit = 0;
        private int _initialBlocks;
        private bool _gameEnded = false;

        public float CurrentScore => _currentScore;

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
            _strikes++;
            _scoreMultiplier = 1.0f;

            Debug.Log("Score deducted!");

            if (_strikes >= 2)
            {
                _lives--;
                _strikes = 0;
                Debug.Log($"Vidas restantes: {_lives}");
            }
            else
            {
                Debug.Log("Strike");
            }

        }
        private void Start()
        {
            _initialBlocks = Object.FindObjectsByType<CarnivalBlock>(FindObjectsSortMode.None).Length;
        }

        public void EndGame()
        {
            float accuracy = _initialBlocks > 0 ? ((float)_blocksHit / _initialBlocks) * 100f : 0f;

            int stars;
            if (accuracy >= 90f) stars = 5;
            else if (accuracy >= 75f) stars = 4;
            else if (accuracy >= 50f) stars = 3;
            else if (accuracy >= 25f) stars = 2;
            else stars = 1;

            Debug.Log($"Game Over, Final Score: {_currentScore}, Precision: {accuracy}%, Stars: {stars}");
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
                Debug.Log("Nivel terminado!");
            }
        }

    }
}
