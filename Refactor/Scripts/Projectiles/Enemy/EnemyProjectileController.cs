using Core;
using Interfaces;
using Obstacles;
using UnityEngine;

namespace Projectiles.Enemy {
    /// <summary>
    /// Rakieta / bomba UFO. Zmiany: brak NullReference, gdy trafiony obiekt z tagiem Player nie ma
    /// VehicleController w rodzicu; pocisk znika po trafieniu gracza (wcześniej leciał dalej i mógł
    /// wywołać śmierć kilka razy); czasy życia efektów w inspektorze; jedno zniszczenie na pocisk.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyProjectileController : MonoBehaviour, IDestroyable {
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private GameObject holeGameObject;
        [Range(0, 10)] [SerializeField] private int thrustForce;
        [SerializeField] private bool createHole;
        [SerializeField, Min(0f)] private float shotDownEffectLifetime = 1f;
        [SerializeField, Min(0f)] private float groundEffectLifetime = 0.2f;

        private Rigidbody2D _rb;
        private bool _exploded;

        private void Awake() => _rb = GetComponent<Rigidbody2D>();

        private void FixedUpdate() => _rb.AddForce(transform.right * thrustForce);

        /// <summary>Zestrzelenie pociskiem gracza.</summary>
        public void Destroyed() => Explode(shotDownEffectLifetime, false);

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag(GameTags.Ground)) {
                Explode(groundEffectLifetime, createHole);
                return;
            }

            if (other.CompareTag(GameTags.Player) && VehicleHitDetection.TryGetVehicle(other, out var vehicle)) {
                vehicle.PlayerDeath();
                Explode(shotDownEffectLifetime, false);
            }
        }

        private void Explode(float effectLifetime, bool spawnHole) {
            if (_exploded) return;
            _exploded = true;

            var position = transform.position;
            if (explosionEffect != null) Destroy(Instantiate(explosionEffect, position, Quaternion.identity), effectLifetime);
            if (spawnHole && holeGameObject != null) Instantiate(holeGameObject, position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
