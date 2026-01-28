using GGJ2026.Core.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2026.Input
{
    [DisallowMultipleComponent]
    public sealed class InputManager : Singleton<InputManager>
    {
        [SerializeField]
        private float deadZone = 0.15f;

        private InputSystem_Actions _inputActions;

        /// <summary>
        /// Horizontal axis input after dead-zone processing and clamped to [-1, 1].
        /// </summary>
        public float HorizontalAxis { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            _inputActions.Player.Move.performed += OnHorizontalMove;
            _inputActions.Player.Move.canceled += OnHorizontalMove;
        }

        private void OnDisable()
        {
            if (Instance != this)
                return;

            _inputActions.Player.Move.performed -= OnHorizontalMove;
            _inputActions.Player.Move.canceled -= OnHorizontalMove;
            _inputActions.Disable();
        }

        private void OnHorizontalMove(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();

            float x = Mathf.Abs(value.x) < deadZone ? 0f : value.x;
            HorizontalAxis = Mathf.Clamp(x, -1f, 1f);
        }
    }
}
