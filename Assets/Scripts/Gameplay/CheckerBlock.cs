using System;
using UnityEngine;

namespace GGJ2026.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class CheckerBlock : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        internal void PlaySound(AudioClip soundClip)
        {
            _audioSource.clip = soundClip;
            _audioSource.Play();
        }
    }
}
