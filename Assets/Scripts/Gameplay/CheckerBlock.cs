using System;
using UnityEngine;
using GGJ2026.Audio;

namespace GGJ2026.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class CheckerBlock : MonoBehaviour
    {
        [Serializable]
        public sealed class ColorSoundSet
        {
            public MaskColors Color = MaskColors.Red;

            [Header("OK clips")]
            public AudioClip[] OkClips;

            [Header("Fail clips")]
            public AudioClip[] FailClips;
        }

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private ColorSoundSet[] _soundSets;
        [SerializeField] private MusicLayerController _musicLayers;
        [SerializeField] private float _playerErrorCooldown = 1.25f;

        private float _playerErrorTimer = 0.0f;

        private void Awake()
        {
            if (_musicLayers == null)
                _musicLayers = FindFirstObjectByType<MusicLayerController>();
        }

        private void Start()
        {
            GameManager.Instance.OnPlayerMistake += OnHandlePlayerMistake;
        }

        private void Update()
        {
            if (_playerErrorTimer < _playerErrorCooldown)
                _playerErrorTimer += Time.deltaTime;
        }

        public void PlayFor(MaskColors color, HitOutcome outcome)
        {
            AudioClip clip = PickClip(color, outcome);
            if (clip == null)
                return;

            if (_musicLayers != null)
            {
                _musicLayers.ScheduleSfxOnBeat(clip);
                return;
            }

            // PlayOneShot evita pisar el clip actual y es más seguro para SFX cortos
            if (_audioSource != null)
                _audioSource.PlayOneShot(clip);
        }

        private AudioClip PickClip(MaskColors color, HitOutcome outcome)
        {
            if (_soundSets == null || _soundSets.Length == 0)
                return null;

            for (int i = 0; i < _soundSets.Length; i++)
            {
                if (_soundSets[i].Color != color)
                    continue;

                AudioClip[] pool = (outcome == HitOutcome.Ok || _playerErrorTimer < _playerErrorCooldown) ? 
                    _soundSets[i].OkClips : _soundSets[i].FailClips;
                if (pool == null || pool.Length == 0)
                    return null;

                int idx = UnityEngine.Random.Range(0, pool.Length);
                return pool[idx];
            }

            return null;
        }

        private void OnHandlePlayerMistake()
        {
            _playerErrorTimer = 0.0f;
        }
    }
}
