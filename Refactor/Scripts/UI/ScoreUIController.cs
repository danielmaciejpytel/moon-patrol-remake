using ScriptableObjects.Score;
using TMPro;
using UnityEngine;

namespace UI {
    /// <summary>
    /// Wynik w HUD. Zmiany: TextMeshPro zamiast legacy UI Text; aktualizacja na event ScoreStoreSO.Changed
    /// zamiast interpolacji stringa w każdej klatce (alokacja GC 60×/s); SetText bez alokacji.
    /// </summary>
    public class ScoreUIController : MonoBehaviour {
        [SerializeField] private TMP_Text text;
        [SerializeField] private ScoreStoreSO scoreStore;
        [SerializeField] private string format = "{0:0000000}";

        private void OnEnable() {
            scoreStore.Changed += Refresh;
            Refresh();
        }

        private void OnDisable() => scoreStore.Changed -= Refresh;

        private void Refresh() => text.SetText(format, scoreStore.PlayerOverallScore);
    }
}
