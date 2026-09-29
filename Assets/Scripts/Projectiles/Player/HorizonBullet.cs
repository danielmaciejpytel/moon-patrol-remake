using Interfaces;
using ScriptableObjects.Projectile;
using UnityEngine;

namespace Projectiles.Player {
    [RequireComponent(typeof(Rigidbody2D))]
    public class HorizonBullet : PooledProjectile {
        [SerializeField] private Rigidbody2D bulletRigidBody;
        [SerializeField] private ProjectileSpeedSO projectileSpeed;

        private void Awake() {
            if (bulletRigidBody == null) bulletRigidBody = GetComponent<Rigidbody2D>();
        }

        public override void Launch(Vector3 position, float lifetime) {
            base.Launch(position, lifetime);
            bulletRigidBody.position = position;
            bulletRigidBody.linearVelocity = new Vector2(projectileSpeed.ProjectileSpeed, 0f);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // Wcześniej pocisk znikał na KAŻDYM triggerze (także na kołach własnego pojazdu i strefach fal).
            if (!other.TryGetComponent<IDestroyable>(out var target)) return;
            target.Destroyed();
            Core.GameEvents.RaiseShotHit();
            Despawn();
        }
    }
}
