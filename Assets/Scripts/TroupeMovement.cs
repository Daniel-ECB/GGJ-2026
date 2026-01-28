using UnityEngine;

namespace GGJ2026.Troupe
{
    [DisallowMultipleComponent]
    public sealed class TroupeMovement : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed = 5.0f;

        private void Start()
        {
            Input.InputManager.Instance.OnHorizontalMovement += HandleHorizontalMovement;
        }

        private void HandleHorizontalMovement(float movement)
        {
            transform.Translate(Vector3.right * movement * _moveSpeed * Time.deltaTime);
        }
    }
}
