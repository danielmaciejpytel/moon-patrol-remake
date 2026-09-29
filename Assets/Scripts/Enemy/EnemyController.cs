using Audio;
using Core;
using Interfaces;
using ScriptableObjects.Enemies;
using UnityEngine;

namespace Enemy {
    /// <summary>
    /// Zmiany: brak FindObjectOfType/GameObject.Find ("ScoreManager", "AiWalls") przy starcie i śmierci,
    /// punkty przez GameEvents, licznik ucieczki bez Debug.Log w pętli, zabezpieczenie przed podwójnym zniszczeniem.
    /// </summary>
    [RequireComponent(typeof(EnemyAI))]
    public class EnemyController : MonoBehaviour, IDestroyable {
        [SerializeField] private EnemyParamsSO enemyParamsSO;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField, Min(0f)] private float explosionLifetime = 4f;
        [Tooltip("Planowany popup z punktami – obecnie nieużywany")]
        [SerializeField] private Transform pfScorePopup;

        private EnemyAI _enemyAi;
        private float _fleeTime;
        private bool _fleeing;
        private bool _destroyed;

        private void Awake() => _enemyAi = GetComponent<EnemyAI>();

        private void Start() => _fleeTime = Time.time + enemyParamsSO.TimeToFlee;

        private void Update() {
            if (_fleeing || Time.time < _fleeTime) return;
            _fleeing = true;
            _enemyAi.EnemyFlee();
        }

        public void Destroyed() {
            if (_destroyed) return;
            _destroyed = true;

            GameEvents.AwardScore(enemyParamsSO.Score);
            GameEvents.RaiseUfoDestroyed();
            AudioManager.Play(SoundId.Hit);

            if (explosionEffect != null) {
                // Rodzic = rodzic UFO (AiWalls), więc eksplozja porusza się razem z kamerą jak wcześniej.
                var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity, transform.parent);
                Destroy(explosion, explosionLifetime);
            }

            Destroy(gameObject);
        }
    }
}
