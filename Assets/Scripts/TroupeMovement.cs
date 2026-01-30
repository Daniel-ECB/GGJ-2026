using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

namespace GGJ2026.Troupe
{
    [DisallowMultipleComponent]
    public sealed class TroupeMovement : MonoBehaviour
    {
        [Header("Movement")]
        
        [SerializeField]
        private float _forwardMoveSpeed = 1.5f;

        [SerializeField]
        private float _maxHorizontalSpeed = 8.0f;   // u/s

        [SerializeField]
        private float _horizontalAccel = 30.0f;     // u/s^2

        [SerializeField]
        private float _horizontalDecel = 40.0f;     // u/s^2

        [SerializeField]
        private float _inputDeadZone = 0.05f;

        [SerializeField]
        private float _halfTrackWidth = 3.0f;

        [Header("Units")]
        [SerializeField]
        private TroupeUnit _leadingUnit = default;

        [SerializeField]
        private Transform _leadingUnitDefaultPos;

        [Header("Audio Sync (Forward Movement)")]
        [Tooltip("If true, forward movement is driven by AudioSettings.dspTime to avoid drift.")]
        [SerializeField]
        private bool _useDspTimeForForward = true;

        [Tooltip("Optional offset (seconds) to align the forward motion start with music start.")]
        [SerializeField]
        private double _dspStartOffsetSeconds = 0.0;

        private double _dspStartTime;
        private float _initialLocalZ;
        private float _vx = 0f;

        private void Start()
        {
            // Cache start position so we can write Z deterministically.
            _initialLocalZ = transform.localPosition.z;

            // Capture the DSP start time. Ideally this should match the moment the music is started.
            _dspStartTime = AudioSettings.dspTime + _dspStartOffsetSeconds;
        }

        private void Update()
        {
            float inputX = Input.InputManager.Instance.HorizontalAxis;

            MoveTroupeForward();
            MoveLeadingUnit(inputX);
        }

        private void MoveTroupeForward()
        {
            if (!_useDspTimeForForward)
            {
                // Original behaviour (can drift over time in rhythm games).
                Vector3 movement = (Vector3.forward * _forwardMoveSpeed) * Time.deltaTime;
                transform.Translate(movement, Space.Self);
                return;
            }

            // Deterministic forward motion derived from DSP time (audio clock).
            double dspNow = AudioSettings.dspTime;

            // If for some reason DSP time is still "before" our start (offset), clamp to 0 elapsed.
            double elapsed = dspNow - _dspStartTime;
            if (elapsed < 0.0)
                elapsed = 0.0;

            float z = _initialLocalZ + (float)(elapsed * _forwardMoveSpeed);

            Vector3 localPos = transform.localPosition;
            localPos.z = z;
            transform.localPosition = localPos;
        }

        private void MoveLeadingUnit(float inputX)
        {
            if (_leadingUnit == null) return;

            // Deadzone para evitar jitter
            if (Mathf.Abs(inputX) < _inputDeadZone)
                inputX = 0f;

            float dt = Time.deltaTime;

            // Velocidad objetivo según input
            float targetVx = inputX * _maxHorizontalSpeed;

            // Elegimos tasa: acelera si hay input, desacelera si no hay
            float rate = (inputX != 0f) ? _horizontalAccel : _horizontalDecel;

            // Mover vx hacia targetVx con aceleración limitada
            _vx = Mathf.MoveTowards(_vx, targetVx, rate * dt);

            Transform t = _leadingUnit.transform;
            Vector3 localPos = t.localPosition;

            // Integración lateral (acá el drift no importa)
            localPos.x += _vx * dt;

            // Clamp
            localPos.x = Mathf.Clamp(localPos.x, -_halfTrackWidth, _halfTrackWidth);

            // Si chocás contra el borde, cortá velocidad para no “pegarse”
            if (localPos.x <= -_halfTrackWidth + 0.0001f || localPos.x >= _halfTrackWidth - 0.0001f)
                _vx = 0f;

            t.localPosition = localPos;
        }

        public void ChangeLeadingUnit(TroupeUnit troupeUnit)
        {
            if (troupeUnit == null)
                return;

            _leadingUnit = troupeUnit;
            _leadingUnit.MoveUnit(_leadingUnitDefaultPos);
            _leadingUnit.tag = "Player";
        }

        /// <summary>
        /// Call this if you restart the run / song and want forward movement to realign.
        /// Useful when entering play mode without reloading the scene, or on retry.
        /// </summary>
        public void ResetDspForwardOrigin()
        {
            _initialLocalZ = transform.localPosition.z;
            _dspStartTime = AudioSettings.dspTime + _dspStartOffsetSeconds;
        }
    }
}
