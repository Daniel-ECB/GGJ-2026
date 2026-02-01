using UnityEngine;
using UnityEngine.UI;
using GGJ2026.Gameplay;
using System.Collections;

namespace GGJ2026.UI
{
    public sealed class MaskUIItem : MonoBehaviour
    {
        [SerializeField] private MaskColors _color;
        [SerializeField] private Animator _animator;
        [SerializeField] private Image _instrumentImage;
        [SerializeField] private Image _failImage;
        [SerializeField] private float _hitDisplayDuration = 0.8f;
        [SerializeField] private float _failDisplayDuration = 0.35f;

        [Header("Animator Params")]
        [SerializeField] private string _activeBool = "Active";
        [SerializeField] private string _hitTrigger = "Hit";
        [SerializeField] private string _failTrigger = "Fail";

        private Coroutine _hitRoutine;
        private Coroutine _failRoutine;

        public MaskColors Color => _color;

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();

            if (_instrumentImage == null)
                _instrumentImage = transform.Find("InstrumentIcon")?.GetComponent<Image>();

            if (_failImage == null)
                _failImage = transform.Find("FailIcon")?.GetComponent<Image>();

            HideAll();
        }

        public void SetActive(bool active)
        {
            if (_animator != null && !string.IsNullOrEmpty(_activeBool))
                _animator.SetBool(_activeBool, active);
        }

        public void PlayHit()
        {
            if (_animator != null && !string.IsNullOrEmpty(_hitTrigger))
                _animator.SetTrigger(_hitTrigger);

            if (_hitRoutine != null)
                StopCoroutine(_hitRoutine);
            if (_failRoutine != null)
                StopCoroutine(_failRoutine);

            _hitRoutine = StartCoroutine(ShowInstrumentTemporarily());
        }

        public void PlayFail()
        {
            if (_animator != null && !string.IsNullOrEmpty(_failTrigger))
                _animator.SetTrigger(_failTrigger);

            if (_failRoutine != null)
                StopCoroutine(_failRoutine);
            if (_hitRoutine != null)
                StopCoroutine(_hitRoutine);

            _failRoutine = StartCoroutine(ShowFailTemporarily());
        }

        private IEnumerator ShowInstrumentTemporarily()
        {
            ShowInstrument();
            yield return new WaitForSeconds(_hitDisplayDuration);
            HideAll();
        }

        private IEnumerator ShowFailTemporarily()
        {
            ShowFail();
            yield return new WaitForSeconds(_failDisplayDuration);
            HideAll();
        }

        private void ShowInstrument()
        {
            if (_instrumentImage != null)
                _instrumentImage.enabled = true;

            if (_failImage != null)
                _failImage.enabled = false;
        }

        private void ShowFail()
        {
            if (_instrumentImage != null)
                _instrumentImage.enabled = false;

            if (_failImage != null)
                _failImage.enabled = true;
        }

        private void HideAll()
        {
            if (_instrumentImage != null)
                _instrumentImage.enabled = false;
            if (_failImage != null)
                _failImage.enabled = false;
        }
    }
}
