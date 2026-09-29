using Core;
using ScriptableObjects.Score;
using UnityEngine;

namespace Score {
    /// <summary>
    /// Jedna instancja na scenę poziomu (prefab ScoreManager). Nasłuchuje GameEvents.ScoreAwarded
    /// i zapisuje do ScoreStoreSO. Zmiany:
    /// - nie zeruje wyniku całkowitego w Start (wcześniej: reset przy KAŻDYM poziomie i przez każdą kopię
    ///   komponentu na kamieniach/minach → "overall score" nie przechodził między etapami);
    ///   reset całej gry robi LevelController.StartGame(), reset etapu – ten komponent;
    /// - bez Debug.Log przy każdym punkcie.
    /// Po migracji: usunąć komponent ScoreManager z prefabów Big Rock, Small Rock i mine.
    /// </summary>
    [DisallowMultipleComponent]
    public class ScoreManager : MonoBehaviour {
        [SerializeField] private ScoreStoreSO scoreStore;

        // Zabezpieczenie na czas migracji: dopóki kopie komponentu siedzą na kamieniach/minach,
        // punkty liczy tylko jedna instancja (pierwsza aktywna; po jej zniszczeniu przejmuje kolejna).
        private static ScoreManager _owner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => _owner = null;

        private void Awake() {
            if (_owner == null) {
                _owner = this;
                scoreStore.ResetForNewStage();
            }
            else if (_owner != this) {
                Debug.LogWarning($"Extra {nameof(ScoreManager)} on {name} – remove it from this prefab.", this);
            }
        }

        private void OnEnable() {
            GameEvents.ScoreAwarded += OnScoreAwarded;
            GameEvents.UfoSpawned += scoreStore.CountUfoSpawned;
            GameEvents.UfoDestroyed += scoreStore.CountUfoDestroyed;
            GameEvents.ShotFired += scoreStore.CountShotFired;
            GameEvents.ShotHit += scoreStore.CountShotHit;
        }

        private void OnDisable() {
            GameEvents.ScoreAwarded -= OnScoreAwarded;
            GameEvents.UfoSpawned -= scoreStore.CountUfoSpawned;
            GameEvents.UfoDestroyed -= scoreStore.CountUfoDestroyed;
            GameEvents.ShotFired -= scoreStore.CountShotFired;
            GameEvents.ShotHit -= scoreStore.CountShotHit;
            if (_owner == this) _owner = null;
        }

        private void OnScoreAwarded(int points) {
            if (_owner == null) _owner = this;
            if (_owner != this) return;
            scoreStore.AddScore(points);
        }
    }
}
