using UnityEngine;

namespace GGJ2026.Troupe
{
    [DisallowMultipleComponent]
    public sealed class TroupeMovement : MonoBehaviour
    {
        [SerializeField]
        private float _horizontalMoveSpeed = 5.0f;
        [SerializeField]
        private float _forwardMoveSpeed = 1.5f;

        private void Update()
        {
            float inputX = Input.InputManager.Instance.HorizontalAxis;
            Vector3 movement = (Vector3.right * (inputX * _horizontalMoveSpeed) + Vector3.forward * _forwardMoveSpeed) * Time.deltaTime;
            transform.Translate(movement, Space.Self);
        }
    }
}
