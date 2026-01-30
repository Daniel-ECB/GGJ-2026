using GGJ2026.Gameplay;
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

        public MaskColors MaskColor => _unitColor;

        public void SetMaskColor(MaskColors color, Material newMaterial)
        {
            if (!gameObject.activeSelf)
                return;

            _unitRenderer.material = newMaterial;
            _unitColor = color;
        }
    }
}
