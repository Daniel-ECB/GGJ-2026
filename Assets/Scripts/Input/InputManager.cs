using GGJ2026.Core.Utils;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2026.Input
{
    [DisallowMultipleComponent]
    public sealed class InputManager : Singleton<InputManager>
    {
        public event Action<float> OnHorizontalMovement;
        // -1 = left, +1 = right

        [SerializeField]
        private float deadZone = 0.4f;

        private InputSystem_Actions _inputActions;
        //private float lastMoveTime;
        //private const float repeatDelay = 0.15f; // prevents stick spam

        protected override void Awake()
        {
            base.Awake();

            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            _inputActions.Player.Move.performed += OnHorizontalMove;
        }

        private void OnDisable()
        {
            _inputActions.Player.Move.performed -= OnHorizontalMove;
            _inputActions.Disable();
        }

        private void OnHorizontalMove(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();

            if (Mathf.Abs(value.x) < deadZone)
                return;

            //if (Time.time - lastMoveTime < repeatDelay)
            //    return;

            //int direction = value.x > 0f ? 1 : -1;
            //lastMoveTime = Time.time;

            //OnHorizontalMovement?.Invoke(direction);
            OnHorizontalMovement?.Invoke(value.x);
        }
    }
}
