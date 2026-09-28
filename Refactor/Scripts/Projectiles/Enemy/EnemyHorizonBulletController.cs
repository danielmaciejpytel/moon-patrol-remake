using Obstacles;
using ScriptableObjects.Projectile;
using UnityEngine;

namespace Projectiles.Enemy {
    /// <summary>
    /// Poziomy pocisk wroga. Zmiany: bez Debug.Log w OnDestroy i przy każdym triggerze
    /// (log z błędnym tekstem "Vertical Bullet hit"), rozpoznanie pojazdu także po kole,
    /// czas życia zamiast lotu w nieskończoność poza ekranem.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyHorizonBulletController : MonoBehaviour {
        [SerializeField] private Rigidbody2D bulletRigidBody;
        [SerializeField] private ProjectileSpeedSO projectileSpeed;
        [SerializeField, Min(0.1f)] private float lifetime = 5f;

        private void Awake() {
            if (bulletRigidBody == null) bulletRigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start() {
            bulletRigidBody.linearVelocity = new Vector2(projectileSpeed.ProjectileSpeed, 0f);
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (VehicleHitDetection.TryGetVehicle(other, out var vehicle)) vehicle.PlayerDeath();
            Destroy(gameObject);
        }
    }
}
