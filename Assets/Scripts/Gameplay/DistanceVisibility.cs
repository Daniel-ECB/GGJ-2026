using UnityEngine;

public sealed class DistanceVisibility : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _visibleDistance = 200f;
    [SerializeField] private Camera _camera;

    private void Awake()
    {
        if (_renderer == null) _renderer = GetComponentInChildren<Renderer>();
        if (_camera == null) _camera = Camera.main;
    }

    private void Update()
    {
        if (_renderer == null || _camera == null) return;
        float dist = Vector3.Distance(_camera.transform.position, transform.position);
        _renderer.enabled = dist <= _visibleDistance;
    }
}