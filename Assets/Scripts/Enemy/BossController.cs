using System.Collections;
using Audio;
using Core;
using Interfaces;
using UnityEngine;

namespace Enemy {
    /// <summary>
    /// Boss na końcu etapu: duży spodek, który krąży nad pojazdem i zrzuca bomby.
    /// Porusza się razem z kamerą (dziecko AiWalls), wytrzymuje kilka trafień pociskami pionowymi.
    /// </summary>
    public class BossController : MonoBehaviour, IDestroyable {
        [SerializeField, Min(1)] private int hitPoints = 12;
        [SerializeField, Min(0)] private int score = 2000;
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private Transform dropPoint;
        [SerializeField] private Vector2 bombInterval = new Vector2(1.2f, 2.2f);
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Tooltip("Wysokość lotu nad środkiem kamery")]
        [SerializeField] private float hoverHeight = 3.2f;
        [SerializeField] private float sweepWidth = 4.5f;
        [SerializeField] private float sweepSpeed = 0.7f;

        private int _hp;
        private bool _destroyed;
        private float _phase;
        private Color _baseColor = Color.white;

        private void Start() {
            _hp = hitPoints;
            if (spriteRenderer != null) _baseColor = spriteRenderer.color;
            GameEvents.ShowToast("WARNING", "MOTHERSHIP APPROACHING", 1.8f);
            AudioManager.Play(SoundId.Enemy);
            StartCoroutine(BombLoop());
        }

        private void Update() {
            // Pozycja lokalna względem AiWalls (porusza się z kamerą): wlot z góry, potem slalom nad drogą.
            _phase += Time.deltaTime * sweepSpeed;
            var target = new Vector3(Mathf.Sin(_phase) * sweepWidth, hoverHeight, transform.localPosition.z);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, 6f * Time.deltaTime);
        }

        private IEnumerator BombLoop() {
            yield return new WaitForSeconds(1.5f);
            while (!_destroyed) {
                if (bombPrefab != null) {
                    var origin = dropPoint != null ? dropPoint.position : transform.position;
                    Instantiate(bombPrefab, origin, Quaternion.identity);
                }

                yield return new WaitForSeconds(Random.Range(bombInterval.x, bombInterval.y));
            }
        }

        public void Destroyed() {
            if (_destroyed) return;
            _hp--;
            AudioManager.Play(SoundId.Hit);
            if (_hp > 0) {
                StartCoroutine(Flash());
                return;
            }

            _destroyed = true;
            Combo.AwardHit(score);
            GameEvents.RaiseUfoDestroyed();
            GameEvents.ShowToast("BOSS DOWN", $"+{score}", 1.8f);
            if (explosionEffect != null)
                for (var i = 0; i < 3; i++) {
                    var p = transform.position + (Vector3)(Random.insideUnitCircle * 0.8f);
                    Destroy(Instantiate(explosionEffect, p, Quaternion.identity, transform.parent), 4f);
                }

            Destroy(gameObject);
        }

        private IEnumerator Flash() {
            if (spriteRenderer == null) yield break;
            spriteRenderer.color = new Color(1f, 0.4f, 0.4f);
            yield return new WaitForSeconds(0.08f);
            spriteRenderer.color = _baseColor;
        }
    }
}
