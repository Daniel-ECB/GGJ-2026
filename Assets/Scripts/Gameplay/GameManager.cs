using GGJ2026.Core.Utils;
using UnityEngine;

namespace GGJ2026.Gameplay
{
    [DisallowMultipleComponent]
    public class GameManager : Singleton<GameManager>
    {
        private float _currentScore = 0.0f;
        private float _scoreMultiplier = 1.0f;

        public float CurrentScore => _currentScore;

        public void ApprovedBlock()
        {
            _currentScore++;
            Debug.Log($"Score added! The current score is {_currentScore}");
        }

        public void FailedBlock()
        {
            Debug.Log("Score deducted!");
        }
    }
}
