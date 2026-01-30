using GGJ2026.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026.Troupe
{
    [DisallowMultipleComponent]
    public sealed class TroupeMasks : MonoBehaviour
    {
        [System.Serializable]
        public class TroupeMaskMaterialPair
        {
            public MaskColors Color;
            public Material Material;
        }

        [SerializeField]
        private float _changeCooldown = 2.0f;
        [SerializeField]
        private TroupeUnit[] _troupeUnits = default;
        [SerializeField]
        private TroupeMaskMaterialPair[] _maskMaterialPairs = default;

        private float _timeSinceLastChange = 0.0f;
        private Dictionary<MaskColors, Material> _maskMaterialsDict = new();

        private void Awake()
        {
            foreach (TroupeMaskMaterialPair pair in _maskMaterialPairs)
            {
                _maskMaterialsDict[pair.Color] = pair.Material;
            }
        }

        private void Update()
        {
            _timeSinceLastChange += Time.deltaTime;
        }

        public void SetMasks(int colorIndex)
        {
            if (_timeSinceLastChange < _changeCooldown)
                return;

            for (int i = 0; i < _troupeUnits.Length; i++)
            {
                _troupeUnits[i].SetMaskColor((MaskColors)colorIndex, _maskMaterialsDict[(MaskColors)colorIndex]);
            }

            _timeSinceLastChange = 0.0f;
        }
    }
}
