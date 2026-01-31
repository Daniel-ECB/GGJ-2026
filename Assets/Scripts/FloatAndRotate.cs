using UnityEngine;

namespace GGJ2026.Core
{
    [DisallowMultipleComponent]
    public sealed class FloatAndRotate : MonoBehaviour
    {
        [Header("Floating")]
        [SerializeField]
        private float _floatAmplitude = 0.35f; // height of the bob
        [SerializeField]
        private float _FloatFrequency = 2f;    // speed of the bob

        [Header("Wobble")]
        [SerializeField]
        private float _wobbleAmplitude = 0.1f;
        [SerializeField]
        private float _wobbleFrequency = 1.5f;

        [Header("Fan like rotation")]
        [SerializeField]
        private float maxYawAngle = 30f;
        [SerializeField]
        private float rotationSpeed = 1.2f;

        private Vector3 _startLocalPos;
        private float _fanTime;
        float _time;

        private void Awake()
        {
            _startLocalPos = transform.localPosition;
        }

        private void Update()
        {
            ApplyWobbleCombined();
            ApplyFanLikeRotation();
        }

        private void ApplyWobbleCombined()
        {
            _time += Time.deltaTime;

            float yOffset = Mathf.Sin(_time * _FloatFrequency) * _floatAmplitude;
            float xOffset = Mathf.Sin(_time * _wobbleFrequency) * _wobbleAmplitude;

            // Start from base and add BOTH offsets before writing once
            Vector3 pos = _startLocalPos;
            pos.y += yOffset;
            pos.x += xOffset;

            transform.localPosition = pos;
        }

        private void ApplyFanLikeRotation()
        {
            _fanTime += Time.deltaTime * rotationSpeed;
            float yaw = Mathf.Sin(_fanTime) * maxYawAngle;
            transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }
}
