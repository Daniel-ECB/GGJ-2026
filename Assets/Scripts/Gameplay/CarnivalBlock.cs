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

        [SerializeField]
        private MaskColors _blockColor = MaskColors.Red;
        [SerializeField]
        private AudioClip _goodBlockSound = default;
        [SerializeField]
        private AudioClip _badBlockSound = default;

        private BlockState _currentBlockState = BlockState.InTransit;
        private bool _hasBeenChecked = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent<IMaskColorReader>(out var colorReader))
                {
                    if (colorReader.MaskColor == _blockColor && !_hasBeenChecked)
                    {
                        _currentBlockState = BlockState.Touched;
                        ShowCorrectTouchFeedback();
                    }
                }
            }
        }

        public void InitializeBlock(MaskColors color, BlockState blockState)
        {
            _blockColor = color;
            _currentBlockState = blockState;
        }

        private void ShowCorrectTouchFeedback()
        {
            // TODO: Visual or audio feedback can be implemented here
        }

        public void TakeHit(Collider collider)
        {
            CheckerBlock checkerBlock = collider.GetComponent<CheckerBlock>();

            if (_currentBlockState == BlockState.Touched)
            {
                checkerBlock.PlaySound(_goodBlockSound);
                GameManager.Instance.ApprovedBlock();
            }
            else if (_currentBlockState == BlockState.InTransit)
            {
                checkerBlock.PlaySound(_badBlockSound);
                GameManager.Instance.FailedBlock();
            }

            Destroy(gameObject);
        }
    }
}
