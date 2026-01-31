using UnityEngine;

namespace GGJ2026.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class CarnivalBlock : MonoBehaviour, IHitController
    {
        [Header("Visuals")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material _redMaterial;
        [SerializeField] private Material _greenMaterial;
        [SerializeField] private Material _blueMaterial;
        [SerializeField] private Material _yellowMaterial;
        [SerializeField] private GameObject _highlightShell;
        [SerializeField] private Renderer _highlightRenderer;
        [SerializeField] private Material _redHighlightMaterial;
        [SerializeField] private Material _greenHighlightMaterial;
        [SerializeField] private Material _blueHighlightMaterial;
        [SerializeField] private Material _yellowHighlightMaterial;
        [SerializeField] private float _highlightDuration = 0.2f;

        public enum BlockState
        {
            InTransit = 0,
            Touched = 1,
            ReachedDestination = 2
        }

        [SerializeField] private MaskColors _blockColor = MaskColors.Red;

        private BlockState _currentBlockState = BlockState.InTransit;
        private bool _hasBeenChecked = false;
        private Coroutine _highlightRoutine;
        private MaskColors _playerColorAtEntry;

        private void Awake()
        {
            ApplyMaterial();
            SetHighlightActive(false);
        }

        private void OnValidate()
        {
            ApplyMaterial();
            SetHighlightActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (!other.TryGetComponent<IMaskColorReader>(out var colorReader))
                return;

            if (_hasBeenChecked)
                return;

            if (colorReader.MaskColor == _blockColor)
            {
                _playerColorAtEntry = colorReader.MaskColor;

                if (!_hasBeenChecked)
                {
                    if (_playerColorAtEntry == _blockColor)
                    {
                        _currentBlockState = BlockState.Touched;
                    }
                    else
                    {
                        _currentBlockState = BlockState.InTransit;
                    }

                    _hasBeenChecked = true;
                    ShowCorrectTouchFeedback();
                }
            }
        }

        public void InitializeBlock(MaskColors color, BlockState blockState)
        {
            _blockColor = color;
            _currentBlockState = blockState;
            ApplyMaterial();
        }

        private void ApplyMaterial()
        {
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            if (_renderer != null)
            {
                Material target = _blockColor switch
                {
                    MaskColors.Red => _redMaterial,
                    MaskColors.Green => _greenMaterial,
                    MaskColors.Blue => _blueMaterial,
                    MaskColors.Yellow => _yellowMaterial,
                    _ => null
                };

                if (target != null)
                    _renderer.sharedMaterial = target;
            }

            ApplyHighlightMaterial();
        }

        private void ApplyHighlightMaterial()
        {
            if (_highlightShell == null)
                return;

            if (_highlightRenderer == null)
                _highlightRenderer = _highlightShell.GetComponentInChildren<Renderer>();

            if (_highlightRenderer == null)
                return;

            Material target = _blockColor switch
            {
                MaskColors.Red => _redHighlightMaterial,
                MaskColors.Green => _greenHighlightMaterial,
                MaskColors.Blue => _blueHighlightMaterial,
                MaskColors.Yellow => _yellowHighlightMaterial,
                _ => null
            };

            if (target != null)
                _highlightRenderer.sharedMaterial = target;
        }

        private void ShowCorrectTouchFeedback()
        {
            if (_highlightShell == null) return;

            if (_highlightRoutine != null)
                StopCoroutine(_highlightRoutine);

            _highlightRoutine = StartCoroutine(HighlightRoutine());
        }

        private System.Collections.IEnumerator HighlightRoutine()
        {
            SetHighlightActive(true);
            yield return new WaitForSeconds(_highlightDuration);
            SetHighlightActive(false);
        }

        private void SetHighlightActive(bool active)
        {
            if (_highlightShell != null)
                _highlightShell.SetActive(active);
        }

        public void TakeHit(Collider collider)
        {
            _hasBeenChecked = true;

            CheckerBlock checkerBlock = collider.GetComponent<CheckerBlock>();
            if (checkerBlock == null)
            {
                Destroy(gameObject);
                return;
            }

            if (_currentBlockState == BlockState.Touched)
            {
                checkerBlock.PlayFor(_blockColor, HitOutcome.Ok);
                GameManager.Instance.ApprovedBlock();
            }
            else if (_currentBlockState == BlockState.InTransit)
            {
                checkerBlock.PlayFor(_blockColor, HitOutcome.Fail);
                GameManager.Instance.FailedBlock();
            }

            Destroy(gameObject);
        }
    }
}
