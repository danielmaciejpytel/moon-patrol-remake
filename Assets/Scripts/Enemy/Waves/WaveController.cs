using System.Collections.Generic;
using Audio;
using UnityEngine;

namespace Enemy.Waves {
    /// <summary>
    /// Obiekt jadący z kamerą, który po wejściu w WaveStarter/WaveManager tworzy falę UFO.
    /// Zmiany:
    /// - namespace "Enemy.WaveS" (literówka) → "Enemy.Waves";
    /// - AiWalls i punkty spawnu jako referencje w inspektorze (lista w prefabie była pusta – działało tylko
    ///   dzięki override w scenie A-E; override ma 4 elementy przy rozmiarze 3);
    /// - bez LINQ Select z efektami ubocznymi (Instantiate + Debug.Log wewnątrz Select);
    /// - sprawdzenie pustej listy zamiast ArgumentOutOfRangeException.
    /// </summary>
    public class WaveController : MonoBehaviour {
        [SerializeField] private Transform aiWalls;
        [SerializeField] private List<GameObject> respawnPointsList = new List<GameObject>(); // typ bez zmian → dane ze scen zostają

        public Transform AiWalls => aiWalls;

        public void WaveRespawn(IReadOnlyList<GameObject> ufoWave) {
            if (respawnPointsList.Count == 0) {
                Debug.LogError($"{nameof(WaveController)}: no respawn points assigned", this);
                return;
            }

            AudioManager.Play(SoundId.Enemy);
            foreach (var ufoPrefab in ufoWave) {
                var point = respawnPointsList[Random.Range(0, respawnPointsList.Count)];
                Instantiate(ufoPrefab, point.transform.position, Quaternion.identity, aiWalls);
            }
        }

        public void SpawnAt(IReadOnlyList<GameObject> ufoWave, Vector3 position) {
            foreach (var ufoPrefab in ufoWave) Instantiate(ufoPrefab, position, Quaternion.identity, aiWalls);
        }
    }
}
