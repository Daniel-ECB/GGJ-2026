using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026.Audio
{
    public sealed class MusicLayerController : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private AudioSource _baseBeat;
        [SerializeField] private AudioSource _redLayer;
        [SerializeField] private AudioSource _greenLayer;
        [SerializeField] private AudioSource _blueLayer;
        [SerializeField] private AudioSource _yellowLayer;

        [Header("Timing")]
        [SerializeField] private float _bpm = 110f;
        [SerializeField] private double _startDelaySec = 0.1;
        [SerializeField] private bool _useDspScheduling = true;

        [Header("SFX Quantized")]
        [SerializeField] private int _sfxPoolSize = 4;
        [SerializeField] private float _sfxVolume = 1.0f;
        [Tooltip("1 = negras, 2 = corcheas, 4 = semicorcheas.")]
        [SerializeField] private int _sfxSubdivision = 1;
        [Tooltip("Minimum lead time before scheduled play (seconds).")]
        [SerializeField] private float _sfxMinLeadTime = 0.02f;
        [Tooltip("If next grid point is farther than this, play immediately (seconds).")]
        [SerializeField] private float _sfxMaxWait = 0.12f;
        [Tooltip("Fixed offset applied to scheduled SFX time (seconds). Negative plays earlier.")]
        [SerializeField] private float _sfxTimeOffsetSec = 0.0f;
        [SerializeField] private AudioSource[] _sfxSources;

        [Header("Mix")]
        [SerializeField] private float _activeVolume = 1.0f;
        [SerializeField] private float _inactiveVolume = 0.0f;
        [SerializeField] private float _crossfadeSec = 0.10f; // 100ms

        private AudioSource[] _layers;
        private float[] _targetVolumes;
        private double _dspStartTime;
        private int _sfxNextIndex = 0;
        private bool _isReady = false;
        private readonly List<AudioClip> _pendingSfx = new();

        public bool IsReady => _isReady;

        private void Awake()
        {
            _layers = new[] { _redLayer, _greenLayer, _blueLayer, _yellowLayer };
            _targetVolumes = new float[_layers.Length];

            if (_sfxSources == null || _sfxSources.Length == 0)
            {
                _sfxSources = new AudioSource[Mathf.Max(1, _sfxPoolSize)];
                for (int i = 0; i < _sfxSources.Length; i++)
                {
                    AudioSource src = gameObject.AddComponent<AudioSource>();
                    src.playOnAwake = false;
                    src.loop = false;
                    src.spatialBlend = 0f;
                    _sfxSources[i] = src;
                }
            }
        }

        private void Start()
        {
            StartAllSynced();
            // Default: rojo (o el que quieras)
            SetMaskColor(GGJ2026.Gameplay.MaskColors.Red, immediate: true);
            Debug.Log($"Red isPlaying: {_redLayer.isPlaying}");
            Debug.Log($"Green isPlaying: {_greenLayer.isPlaying}");
            Debug.Log($"Blue isPlaying: {_blueLayer.isPlaying}");
            Debug.Log($"Yellow isPlaying: {_yellowLayer.isPlaying}");

            // Only for testing, setting all layers active
            _redLayer.volume = 0.5f;
            _greenLayer.volume = 0.5f;
            _blueLayer.volume = 0.5f;
            _yellowLayer.volume = 0.5f;
        }

        private void Update()
        {
            // Crossfade suave hacia los target volumes
            if (_crossfadeSec <= 0f) return;

            float step = Time.deltaTime / _crossfadeSec;
            for (int i = 0; i < _layers.Length; i++)
            {
                if (_layers[i] == null) continue;
                _layers[i].volume = Mathf.MoveTowards(_layers[i].volume, _targetVolumes[i], step * _activeVolume);
            }
        }

        private void StartAllSynced()
        {
            _dspStartTime = AudioSettings.dspTime + _startDelaySec;

            // Base beat
            if (_baseBeat != null)
            {
                _baseBeat.loop = true;
                if (_useDspScheduling) _baseBeat.PlayScheduled(_dspStartTime);
                else _baseBeat.Play();
            }

            // Color layers
            for (int i = 0; i < _layers.Length; i++)
            {
                if (_layers[i] == null) continue;

                _layers[i].loop = true;
                _layers[i].volume = _inactiveVolume; // arrancan muteadas

                if (_useDspScheduling) _layers[i].PlayScheduled(_dspStartTime);
                else _layers[i].Play();
            }

            _isReady = true;

            if (_pendingSfx.Count > 0)
            {
                for (int i = 0; i < _pendingSfx.Count; i++)
                    ScheduleSfxInternal(_pendingSfx[i]);
                _pendingSfx.Clear();
            }
        }

        public void SetMaskColor(GGJ2026.Gameplay.MaskColors color, bool immediate = false)
        {
            int idx = ColorToIndex(color);

            for (int i = 0; i < _layers.Length; i++)
            {
                _targetVolumes[i] = (i == idx) ? _activeVolume : _inactiveVolume;

                if (immediate && _layers[i] != null)
                    _layers[i].volume = _targetVolumes[i];
            }
        }

        private int ColorToIndex(GGJ2026.Gameplay.MaskColors c)
        {
            // Enum: Red=0, Green=1, Blue=2, Yellow=3
            return (int)c;
        }

        public void ScheduleSfxOnBeat(AudioClip clip)
        {
            if (clip == null || _sfxSources == null || _sfxSources.Length == 0)
                return;

            if (!_isReady)
            {
                Debug.Log($"[MusicLayerController] SFX queued before ready: {clip.name}");
                _pendingSfx.Add(clip);
                return;
            }

            ScheduleSfxInternal(clip);
        }

        private void ScheduleSfxInternal(AudioClip clip)
        {
            double now = AudioSettings.dspTime;
            if (_dspStartTime <= 0.0)
                _dspStartTime = now;

            int subdivision = Mathf.Max(1, _sfxSubdivision);
            double beatDur = 60.0 / Mathf.Max(1.0f, _bpm);
            double step = beatDur / subdivision;

            // Schedule to nearest grid point, but avoid long waits.
            double elapsed = now - _dspStartTime;
            if (elapsed < 0.0) elapsed = 0.0;
            double rawSteps = elapsed / step;
            double nearestSteps = System.Math.Round(rawSteps);
            double scheduled = _dspStartTime + (nearestSteps * step);

            // If nearest is in the past or too close, move forward.
            if (scheduled < now + _sfxMinLeadTime)
                scheduled = _dspStartTime + (System.Math.Ceiling(rawSteps) * step);

            // If next grid is too far, play immediately.
            if (scheduled - now > _sfxMaxWait)
                scheduled = now + _sfxMinLeadTime;

            scheduled += _sfxTimeOffsetSec;
            if (scheduled < now + _sfxMinLeadTime)
                scheduled = now + _sfxMinLeadTime;

            AudioSource src = _sfxSources[_sfxNextIndex % _sfxSources.Length];
            _sfxNextIndex++;

            src.volume = _sfxVolume;
            src.clip = clip;
            src.PlayScheduled(scheduled);
        }
    }
}
