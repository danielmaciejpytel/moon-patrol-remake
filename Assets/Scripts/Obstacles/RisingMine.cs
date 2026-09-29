using UnityEngine;

namespace Obstacles {
    /// <summary>
    /// Mina wynurzająca się z ziemi: schowana pod drogą, wysuwa się, gdy pojazd się zbliży.
    /// Kolizja działa dopiero po wysunięciu, więc przy dużej prędkości da się ją minąć, zanim wyjdzie.
    /// </summary>
    public class RisingMine : MonoBehaviour {
        [SerializeField] private float triggerDistance = 7f;
        [SerializeField] private float depth = 0.6f;
        [SerializeField, Min(0.05f)] private float riseTime = 0.35f;
        [SerializeField] private Collider2D[] colliders;

        private Transform _vehicle;
        private Vector3 _restPosition;
        private float _progress = -1f;
        private Renderer[] _renderers;

        private void Start() {
            _restPosition = transform.position;
            transform.position = _restPosition + Vector3.down * depth;
            if (colliders == null || colliders.Length == 0) colliders = GetComponentsInChildren<Collider2D>();
            SetColliders(false);
            _renderers = GetComponentsInChildren<Renderer>();
            foreach (var r in _renderers) r.enabled = false;
            var vehicle = FindFirstObjectByType<Vehicle.VehicleController>();
            if (vehicle != null) _vehicle = vehicle.transform;
        }

        private void Update() {
            if (_progress >= 1f || _vehicle == null) return;
            if (_progress < 0f) {
                var dx = transform.position.x - _vehicle.position.x;
                if (dx > triggerDistance || dx < 0f) return;
                _progress = 0f;
                foreach (var r in _renderers) r.enabled = true;
            }

            _progress = Mathf.Min(1f, _progress + Time.deltaTime / riseTime);
            transform.position = _restPosition + Vector3.down * (depth * (1f - _progress));
            if (_progress >= 0.8f) SetColliders(true);
        }

        private void SetColliders(bool value) {
            foreach (var c in colliders) if (c != null) c.enabled = value;
        }
    }
}
