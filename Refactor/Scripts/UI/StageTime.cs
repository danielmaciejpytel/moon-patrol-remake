using ScriptableObjects.Score;
using TMPro;
using UnityEngine;

namespace UI {
    /// <summary>
    /// Czas etapu. Zmiany: TextMeshPro; tekst i SO aktualizowane tylko przy zmianie sekundy
    /// (wcześniej ToString("000") co klatkę); licznik zatrzymuje się po śmierci gracza.
    /// </summary>
    public class StageTime : MonoBehaviour {
        [SerializeField] private ScoreStoreSO scoreStore;
        [SerializeField] private TMP_Text stageTimer;

        private float _elapsed;
        private int _shownSeconds = -1;
        private bool _stopped;

        private void OnEnable() => Core.GameEvents.PlayerDied += Stop;

        private void OnDisable() => Core.GameEvents.PlayerDied -= Stop;

        private void Stop() => _stopped = true;

        private void Update() {
            if (_stopped) return;
            _elapsed += Time.deltaTime;

            var seconds = Mathf.FloorToInt(_elapsed);
            if (seconds == _shownSeconds) return;
            _shownSeconds = seconds;

            scoreStore.SetStageTime(seconds);
            stageTimer.SetText("{0:000}", seconds);
        }
    }
}
