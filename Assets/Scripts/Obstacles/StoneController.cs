using Core;
using Interfaces;
using ScriptableObjects.Obstacles;
using UnityEngine;

namespace Obstacles {
    /// <summary>
    /// Kamień. Naprawione:
    /// - warunek "vehicleController != null || vehicleTire" wywoływał PlayerDeath() na null;
    /// - GetComponent&lt;ScoreManager&gt;() na kamieniu (ScoreManager był doklejony do każdego kamienia i miny,
    ///   a każdy z nich w Start zerował wynik gracza) → GameEvents.AwardScore;
    /// - brak zabezpieczenia przed podwójnym zniszczeniem (dwa pociski w jednej klatce = podwójne punkty).
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class StoneController : MonoBehaviour, IDestroyable {
        [SerializeField] private ObstaclesParamsSO rockParams;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField, Min(0f)] private float explosionLifetime = 4f;

        private bool _destroyed;

        private void Awake() {
            if (rockParams.ObstacleSprite != null) GetComponent<SpriteRenderer>().sprite = rockParams.ObstacleSprite;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (VehicleHitDetection.TryGetVehicle(other, out var vehicle)) vehicle.PlayerDeath();
        }

        public void Destroyed() {
            if (_destroyed) return;
            _destroyed = true;

            Combo.AwardHit(rockParams.DestroyScore);

            if (explosionEffect != null) {
                var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, explosionLifetime);
            }
            else {
                Debug.LogWarning($"{name} missing explosion effect", this);
            }

            Destroy(gameObject);
        }
    }
}
