using Interfaces;
using ScriptableObjects.Projectile;
using UnityEngine;

namespace Projectiles.Player {
    /// <summary>
    /// Pocisk pionowy podąża w osi X za punktem strzału pojazdu (jak w oryginale).
    /// Zmiany: Rigidbody2D skonfigurowany w prefabie (Kinematic, Continuous) zamiast AddComponent w Start,
    /// usunięte mass = float.MinValue (wartość ujemna, nieprawidłowa), isKinematic (przestarzałe) → bodyType,
    /// ProjectileSpeedSO przypisany w inspektorze zamiast Resources.Load, ruch w FixedUpdate przez MovePosition.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class VerticalBullet : PooledProjectile, IDestroyable {
        [SerializeField] private ProjectileSpeedSO projectileSpeed;

        private Rigidbody2D _body;
        private Transform _firePoint;

        private void Awake() {
            _body = GetComponent<Rigidbody2D>();
            _body.bodyType = RigidbodyType2D.Kinematic;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void SetFirePoint(Transform firePoint) => _firePoint = firePoint;

        public override void Launch(Vector3 position, float lifetime) {
            base.Launch(position, lifetime);
            _body.position = position;
        }

        private void FixedUpdate() {
            var x = _firePoint != null ? _firePoint.position.x : _body.position.x;
            var y = _body.position.y + projectileSpeed.ProjectileSpeed * Time.fixedDeltaTime;
            _body.MovePosition(new Vector2(x, y));
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.TryGetComponent<IDestroyable>(out var target)) return;
            target.Destroyed();
            Despawn();
        }

        public void Destroyed() => Despawn();
    }
}
