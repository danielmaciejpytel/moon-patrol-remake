using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Waves {
    /// <summary>
    /// Wyzwalacz fali. Łączy WaveStarter (spawn w losowych punktach WaveController) i WaveManager
    /// (spawn w pozycji wyzwalacza) – różniły się tylko miejscem spawnu.
    /// Rozpoznanie WaveController po komponencie zamiast po nazwie obiektu ("WaveController").
    /// </summary>
    public class WaveStarter : MonoBehaviour {
        public enum SpawnMode {
            RandomRespawnPoints,
            AtTriggerPosition
        }

        [SerializeField] private List<GameObject> ufoWave = new List<GameObject>();
        [SerializeField] private SpawnMode spawnMode = SpawnMode.RandomRespawnPoints;

        private bool _started;

        protected virtual SpawnMode Mode => spawnMode;

        private void OnTriggerEnter2D(Collider2D other) {
            if (_started || !other.TryGetComponent<WaveController>(out var waveController)) return;
            _started = true;

            if (Mode == SpawnMode.RandomRespawnPoints) waveController.WaveRespawn(ufoWave);
            else waveController.SpawnAt(ufoWave, transform.position);
        }
    }
}
