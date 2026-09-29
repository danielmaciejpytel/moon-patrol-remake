using System.Collections;
using Core;
using Interfaces;
using UnityEngine;

namespace Enemy {
    /// <summary>
    /// Czołg z tyłu ekranu: jedzie po drodze przy lewej krawędzi kadru i strzela poziomo w stronę pojazdu
    /// (pocisk trzeba przeskoczyć). Po kilku strzałach zostaje w tyle.
    /// </summary>
    public class TankController : MonoBehaviour, IDestroyable {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField, Min(1)] private int shots = 4;
        [SerializeField] private Vector2 shotInterval = new Vector2(2f, 3f);
        [Tooltip("Odległość od lewej krawędzi kadru")]
        [SerializeField] private float screenMargin = 1.2f;
        [SerializeField] private float groundY = -4.6f;
        [SerializeField, Min(0)] private int score = 300;
        [SerializeField] private GameObject explosionEffect;

        private UnityEngine.Camera _camera;
        private float _offset = -3f;
        private bool _leaving;
        private bool _destroyed;

        private void Start() {
            _camera = UnityEngine.Camera.main;
            var vehicle = FindFirstObjectByType<Vehicle.VehicleController>();
            if (vehicle != null) groundY = vehicle.transform.position.y - 0.1f;
            GameEvents.ShowToast("TANK", "BEHIND YOU · JUMP THE SHELLS", 1.6f);
            StartCoroutine(FireLoop());
        }

        private void LateUpdate() {
            if (_camera == null) return;
            var halfWidth = _camera.orthographicSize * _camera.aspect;
            var targetOffset = _leaving ? -4f : screenMargin;
            _offset = Mathf.MoveTowards(_offset, targetOffset, 2f * Time.deltaTime);
            transform.position = new Vector3(_camera.transform.position.x - halfWidth + _offset, groundY, transform.position.z);
            if (_leaving && Mathf.Approximately(_offset, targetOffset)) Destroy(gameObject);
        }

        private IEnumerator FireLoop() {
            yield return new WaitForSeconds(1.5f);
            for (var i = 0; i < shots && !_destroyed; i++) {
                if (bulletPrefab != null) Instantiate(bulletPrefab, firePoint != null ? firePoint.position : transform.position, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(shotInterval.x, shotInterval.y));
            }

            _leaving = true;
        }

        /// <summary>Trafienie pociskiem pionowym (gdy czołg jest pod pojazdem w skoku).</summary>
        public void Destroyed() {
            if (_destroyed) return;
            _destroyed = true;
            Combo.AwardHit(score);
            if (explosionEffect != null) Destroy(Instantiate(explosionEffect, transform.position, Quaternion.identity), 4f);
            Destroy(gameObject);
        }
    }
}
