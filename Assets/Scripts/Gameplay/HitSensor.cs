using UnityEngine;

namespace GGJ2026.Gameplay
{
    public sealed class HitSensor : MonoBehaviour
    {
        [SerializeField]
        private string _tagToDetect = "CheckerActor";

        private IHitController _hitControllerOwner;

        private void Awake()
        {
            _hitControllerOwner = GetComponentInParent<IHitController>();

            if (_hitControllerOwner == null)
            {
                Debug.LogError("HitSensor could not find an IHitController in its parent hierarchy.", this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_tagToDetect))
            {
                if (_hitControllerOwner != null)
                    _hitControllerOwner.TakeHit(other);
            }
        }
    }
}
