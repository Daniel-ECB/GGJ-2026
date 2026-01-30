using System;
using UnityEngine;

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

        public void PlayFor(MaskColors color, HitOutcome outcome)
        {
            AudioClip clip = PickClip(color, outcome);
            if (clip == null || _audioSource == null)
                return;

            // PlayOneShot evita pisar el clip actual y es más seguro para SFX cortos
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

                AudioClip[] pool = (outcome == HitOutcome.Ok) ? _soundSets[i].OkClips : _soundSets[i].FailClips;
                if (pool == null || pool.Length == 0)
                    return null;

                int idx = UnityEngine.Random.Range(0, pool.Length);
                return pool[idx];
            }

            return null;
        }
    }
}
