using UnityEngine;

namespace GGJ2026.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class CarnivalBlock : MonoBehaviour, IHitController
    {
        public enum BlockState
        {
            InTransit = 0,
            Touched = 1,
            ReachedDestination = 2
        }

        [SerializeField] private MaskColors _blockColor = MaskColors.Red;

        private BlockState _currentBlockState = BlockState.InTransit;
        private bool _hasBeenChecked = false;

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
