using GGJ2026.Gameplay;
using UnityEngine;

namespace GGJ2026.Troupe
{
    [DisallowMultipleComponent]
    public sealed class TroupeUnit : MonoBehaviour
    {
        [SerializeField]
        private MaskColors _unitColor = MaskColors.Red;

        public MaskColors UnitColor => _unitColor;
    }
}
