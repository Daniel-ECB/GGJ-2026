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

        private BlockState _currentBlockState = BlockState.InTransit;
        private bool _hasBeenChecked = false;
        private MaskColors _playerColorAtEntry;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent<IMaskColorReader>(out var colorReader))
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
                        ShowFeedback();
                    }
                }
            }
        }

        public void InitializeBlock(MaskColors color, BlockState blockState)
        {
            _blockColor = color;
            _currentBlockState = blockState;
        }

        private void ShowFeedback()
        {
            // TODO: Visual or audio feedback can be implemented here
        }

        public void TakeHit()
        {
            if (_currentBlockState == BlockState.Touched)
                GameManager.Instance.ApprovedBlock();
            else if (_currentBlockState == BlockState.InTransit)
                GameManager.Instance.FailedBlock();

            Destroy(gameObject);
        }
    }
}