using System.Collections.Generic;
using UnityEngine;
using GGJ2026.Gameplay;
using GGJ2026.Troupe;

namespace GGJ2026.UI
{
    public sealed class MaskUIController : MonoBehaviour
    {
        [SerializeField] private bool _autoFindItemsInChildren = true;
        [SerializeField] private MaskUIItem[] _items;

        private readonly Dictionary<MaskColors, MaskUIItem> _byColor = new();
        private TroupeMasks _troupeMasks;
        private GameManager _gameManager;

        private void Awake()
        {
            if (_autoFindItemsInChildren)
                _items = GetComponentsInChildren<MaskUIItem>(true);

            _byColor.Clear();
            if (_items != null)
            {
                foreach (var item in _items)
                {
                    if (item == null) continue;
                    if (_byColor.ContainsKey(item.Color))
                    {
                        Debug.LogWarning($"[MaskUIController] Duplicate color: {item.Color}");
                        continue;
                    }
                    _byColor.Add(item.Color, item);
                }
            }

            _troupeMasks = FindFirstObjectByType<TroupeMasks>();
            _gameManager = FindFirstObjectByType<GameManager>();
        }

        private void OnEnable()
        {
            if (_gameManager == null)
                _gameManager = FindFirstObjectByType<GameManager>();
            if (_gameManager != null)
                _gameManager.OnBlockResolved += HandleBlockResolved;

            if (_troupeMasks == null)
                _troupeMasks = FindFirstObjectByType<TroupeMasks>();

            if (_troupeMasks != null)
                _troupeMasks.OnMaskChanged += HandleMaskChanged;
        }

        private void Start()
        {
            if (_troupeMasks != null)
                HandleMaskChanged(_troupeMasks.CurrentMaskColor);
        }

        private void OnDisable()
        {
            if (_gameManager != null)
                _gameManager.OnBlockResolved -= HandleBlockResolved;

            if (_troupeMasks != null)
                _troupeMasks.OnMaskChanged -= HandleMaskChanged;
        }

        private void HandleBlockResolved(MaskColors color, HitOutcome outcome)
        {
            if (!_byColor.TryGetValue(color, out var item))
                return;

            if (outcome == HitOutcome.Ok)
                item.PlayHit();
            else
                item.PlayFail();
        }

        private void HandleMaskChanged(MaskColors color)
        {
            foreach (var kv in _byColor)
                kv.Value.SetActive(kv.Key == color);
        }
    }
}
