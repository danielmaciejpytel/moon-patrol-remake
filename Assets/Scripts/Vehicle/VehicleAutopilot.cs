using System.Collections.Generic;
using Enemy;
using Obstacles;
using UnityEngine;

namespace Vehicle {
    /// <summary>
    /// Autopilot pojazdu (menu i tryb demo): przeskakuje dziury i miny, strzela do kamieni i UFO,
    /// przyspiesza przed dziurami. Działa tylko, gdy VehicleController ma włączony autopilot.
    /// </summary>
    [RequireComponent(typeof(VehicleController))]
    public class VehicleAutopilot : MonoBehaviour {
        [SerializeField] private VehicleWeaponController weapon;
        [SerializeField, Min(1f)] private float lookAhead = 9f;
        [Tooltip("Odległość przedniego koła od krawędzi dziury/miny, przy której następuje skok")]
        [SerializeField] private float jumpDistanceHole = 0.35f;
        [SerializeField] private float jumpDistanceRock = 0.9f;
        [SerializeField] private float shootDistance = 7f;
        [SerializeField, Min(0.05f)] private float fireInterval = 0.35f;
        [Tooltip("Przed dziurą szerszą niż ta wartość autopilot przyspiesza")]
        [SerializeField] private float wideHole = 2.2f;

        private VehicleController _vehicle;
        private VehicleTireController[] _tires;
        private readonly List<Obstacle> _obstacles = new List<Obstacle>();
        private int _obstacleFrame = -1;
        private float _nextFire;

        private void Awake() {
            _vehicle = GetComponent<VehicleController>();
            _tires = GetComponentsInChildren<VehicleTireController>();
            if (weapon == null) weapon = GetComponentInChildren<VehicleWeaponController>();
        }

        private void FixedUpdate() {
            if (!_vehicle.IsAutopilot || _vehicle.IsDead) return;

            var front = float.MinValue;
            var back = float.MaxValue;
            foreach (var t in _tires) {
                front = Mathf.Max(front, t.transform.position.x);
                back = Mathf.Min(back, t.transform.position.x);
            }

            var throttle = 0f;
            var maxX = front + lookAhead;
            foreach (var obstacle in Obstacles()) {
                if (obstacle.Collider == null || !obstacle.Collider.enabled) continue;
                var bounds = obstacle.Collider.bounds;
                if (bounds.max.x < back - 1f || bounds.min.x > maxX) continue;
                var distance = bounds.min.x - front;
                if (obstacle.IsRock) {
                    if (distance > 0f && distance < shootDistance) TryFire();
                    if (distance > 0f && distance < jumpDistanceRock) TryJump();
                }
                else {
                    if (bounds.size.x > wideHole && distance > 0f && distance < 5f) throttle = 1f;
                    if (distance > 0f && distance < jumpDistanceHole + bounds.size.x * 0.1f) TryJump();
                }
            }

            foreach (var enemy in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
                if (Mathf.Abs(enemy.transform.position.x - transform.position.x) < 2.5f) TryFire();

            _vehicle.AutopilotThrottle = throttle;
        }

        private struct Obstacle {
            public Collider2D Collider;
            public bool IsRock;
        }

        // Przeszkody zbierane raz na sekundę – nowe mogą powstać w trakcie (kratery po bombach).
        private List<Obstacle> Obstacles() {
            if (_obstacleFrame >= 0 && Time.frameCount - _obstacleFrame < 60) return _obstacles;
            _obstacleFrame = Time.frameCount;
            _obstacles.Clear();
            foreach (var o in FindObjectsByType<ObstaclesController>(FindObjectsSortMode.None))
                if (o.TryGetComponent<Collider2D>(out var c)) _obstacles.Add(new Obstacle { Collider = c });
            foreach (var s in FindObjectsByType<StoneController>(FindObjectsSortMode.None))
                if (s.TryGetComponent<Collider2D>(out var c)) _obstacles.Add(new Obstacle { Collider = c, IsRock = true });
            return _obstacles;
        }

        private void TryJump() {
            if (_vehicle.CanJump) _vehicle.RequestJump();
        }

        private void TryFire() {
            if (weapon == null || Time.time < _nextFire) return;
            _nextFire = Time.time + fireInterval;
            weapon.FireFromCode();
        }
    }
}
