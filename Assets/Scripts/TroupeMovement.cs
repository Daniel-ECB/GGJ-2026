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
        [SerializeField]
        private float _halfTrackWidth = 3.0f;

        [SerializeField]
        private TroupeUnit _leadingUnit = default;
        [SerializeField]
        private Transform _leadingUnitDefaultPos;

        private void Update()
        {
            float inputX = Input.InputManager.Instance.HorizontalAxis;
            MoveTroupe();
            MoveLeadingUnit(inputX);
        }

        private void MoveTroupe()
        {
            Vector3 movement = (Vector3.forward * _forwardMoveSpeed) * Time.deltaTime;
            transform.Translate(movement, Space.Self);
        }

        private void MoveLeadingUnit(float inputX)
        {
            Transform t = _leadingUnit.transform;
            Vector3 localPos = t.localPosition;
            localPos.x += inputX * _horizontalMoveSpeed * Time.deltaTime;
            localPos.x = Mathf.Clamp(localPos.x, -_halfTrackWidth, _halfTrackWidth);
            t.localPosition = localPos;
        }

        public void ChangeLeadingUnit(TroupeUnit troupeUnit)
        {
            _leadingUnit = troupeUnit;
            //_leadingUnit.transform.position = _leadingUnitDefaultPos.position;
            //_leadingUnit.MoveUnit(_leadingUnitDefaultPos.position);
        }
    }
}
