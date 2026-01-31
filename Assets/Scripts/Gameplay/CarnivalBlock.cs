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

        public enum BlockState
        {
            InTransit = 0,
            Touched = 1,
            ReachedDestination = 2
        }

        [SerializeField] private MaskColors _blockColor = MaskColors.Red;

        private BlockState _currentBlockState = BlockState.InTransit;
        private bool _hasBeenChecked = false;

        private void Awake()
        {
            ApplyMaterial();
        }

        private void OnValidate()
        {
            ApplyMaterial();
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
                _currentBlockState = BlockState.Touched;
                ShowCorrectTouchFeedback();
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

            if (_renderer == null)
                return;

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

        private void ShowCorrectTouchFeedback()
        {
            // TODO: feedback visual (brillo, partículas, etc.)
        }

        public void TakeHit(Collider collider)
        {
            if (_hasBeenChecked)
                return;

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
