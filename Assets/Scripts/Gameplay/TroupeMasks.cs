using GGJ2026.Gameplay;
using System.Collections.Generic;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using GGJ2026.Audio;

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

        [Header("Troupe management")]
        [SerializeField]
        private List<TroupeUnit> _troupeUnits = default;
        [SerializeField]
        private TroupeUnit _leadingUnit = default;
        [SerializeField]
        private TroupeMovement _troupeMovement = default;

        [Header("Audio Layers")]
        [SerializeField] private MusicLayerController _musicLayers;

        [Header("Mask Change")]
        [SerializeField]
        private float _changeCooldown = 2.0f;
        [SerializeField]
        private TroupeMaskMaterialPair[] _maskMaterialPairs = default;

        private float _timeSinceLastChange = Mathf.Infinity;
        private Dictionary<MaskColors, Material> _maskMaterialsDict = new();

        private int? _pendingColorIndex = null;

        private void Awake()
        {
            foreach (TroupeMaskMaterialPair pair in _maskMaterialPairs)
            {
                _maskMaterialsDict[pair.Color] = pair.Material;
            }
        }

        private void Start()
        {
            GameManager.Instance.OnPlayerMistake += OnHandlePlayerMistake;
			_timeSinceLastChange = _changeCooldown;

            // In here we could define the starting mask track sound
            // If not the default is red, defined at MusicLayerController
            if (_musicLayers == null)
                _musicLayers = FindFirstObjectByType<MusicLayerController>();
        }

        private void Update()
        {
            _timeSinceLastChange += Time.deltaTime;

            if (UnityInput.GetKeyDown(KeyCode.Z)) TrySetMasks(0);
            if (UnityInput.GetKeyDown(KeyCode.X)) TrySetMasks(1);
            if (UnityInput.GetKeyDown(KeyCode.C)) TrySetMasks(2);
            if (UnityInput.GetKeyDown(KeyCode.V)) TrySetMasks(3);

            if (_pendingColorIndex.HasValue && _timeSinceLastChange >= _changeCooldown)
            {
                SetMasks(_pendingColorIndex.Value);
                _pendingColorIndex = null;
            }
        }

        public void TrySetMasks(int colorIndex)
        {
            if (_timeSinceLastChange < _changeCooldown)
            {
                if (!_pendingColorIndex.HasValue)
                    _pendingColorIndex = colorIndex;
                return;
            }
            SetMasks(colorIndex);
        }

        public void SetMasks(int colorIndex)
        {
            if (_timeSinceLastChange < _changeCooldown)
                return;

            for (int i = 0; i < _troupeUnits.Count; i++)
            {
                _troupeUnits[i].SetMaskColor((MaskColors)colorIndex, _maskMaterialsDict[(MaskColors)colorIndex]);
            }
            
            // Change the track beat by mask, by calling MusicLayerController
            _musicLayers?.SetMaskColor((MaskColors)colorIndex);
            _timeSinceLastChange = 0.0f;
        }

        private void OnHandlePlayerMistake()
        {
            _leadingUnit.ReleaseUnit();
            List<TroupeUnit> candidates = _troupeUnits.FindAll(u => u != null && u != _leadingUnit && u.gameObject.activeSelf);

            if (candidates.Count == 0)
            {
                Debug.LogWarning("No active troupe units available to become the new leading unit.");
                return;
            }

            _leadingUnit = candidates[Random.Range(0, candidates.Count)];
            _troupeMovement.ChangeLeadingUnit(_leadingUnit);
        }
    }
}