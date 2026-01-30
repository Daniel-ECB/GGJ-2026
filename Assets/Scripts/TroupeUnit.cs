using GGJ2026.Gameplay;
using System.Collections;
using UnityEngine;

namespace GGJ2026.Troupe
{
    [DisallowMultipleComponent]
    public sealed class TroupeUnit : MonoBehaviour, IMaskColorReader
    {
        [SerializeField]
        private MaskColors _unitColor = MaskColors.Red;
        [SerializeField]
        private Renderer _unitRenderer = default;

        [Header("Follow / Reposition")]
        [SerializeField]
        private float followSpeed = 6f;
        [SerializeField]
        private float arriveEpsilon = 0.02f;

        private Coroutine _moveRoutine;

        public MaskColors MaskColor => _unitColor;

        public void SetMaskColor(MaskColors color, Material newMaterial)
        {
            if (!gameObject.activeSelf)
                return;

            _unitRenderer.material = newMaterial;
            _unitColor = color;
        }

        public void MoveUnit(Transform destinationTransform)
        {
            if (destinationTransform == null)
            {
                Debug.LogWarning($"{name}: MoveUnit called with null destinationTransform.");
                return;
            }

            if (_moveRoutine != null)
                StopCoroutine(_moveRoutine);

            _moveRoutine = StartCoroutine(MoveUnitRoutine(destinationTransform));
        }

        public void ReleaseUnit()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator MoveUnitRoutine(Transform destinationTransform)
        {
            float epsSqr = arriveEpsilon * arriveEpsilon;

            while (destinationTransform != null)
            {
                Vector3 current = transform.position;
                Vector3 target = destinationTransform.position;

                float step = followSpeed * Time.deltaTime;
                Vector3 next = Vector3.MoveTowards(current, target, step);

                transform.position = next;

                if ((target - next).sqrMagnitude <= epsSqr)
                    break;

                yield return null;
            }

            _moveRoutine = null;
        }
    }
}
