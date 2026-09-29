using UnityEngine;

namespace Enemy.Waves {
    /// <summary>
    /// Wyzwalacz pojedynczego przeciwnika (boss na końcu etapu, czołg z tyłu ekranu). Działa jak WaveStarter:
    /// reaguje na WaveController jadący razem z kamerą.
    /// </summary>
    public class EnemySpawnTrigger : MonoBehaviour {
        [SerializeField] private GameObject enemyPrefab;
        [Tooltip("Boss: dziecko AiWalls (porusza się z kamerą). Czołg: obiekt w świecie, sam pilnuje pozycji.")]
        [SerializeField] private bool parentToAiWalls = true;
        [Tooltip("Pozycja startowa względem AiWalls (dla parentToAiWalls)")]
        [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 9f, 0f);

        private bool _started;

        private void OnTriggerEnter2D(Collider2D other) {
            if (_started || enemyPrefab == null || !other.TryGetComponent<WaveController>(out var waveController)) return;
            _started = true;
            if (parentToAiWalls && waveController.AiWalls != null) {
                var enemy = Instantiate(enemyPrefab, waveController.AiWalls);
                enemy.transform.localPosition = spawnOffset;
            }
            else {
                Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
