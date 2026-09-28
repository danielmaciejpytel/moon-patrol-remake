using UnityEngine;
using UnityEngine.UI;

namespace UI {
    /// <summary>
    /// Jeden pasek postępu zamiast trzech prawie identycznych klas LevelAEProgressBar / LevelEJProgressBar /
    /// LevelJOProgress (różniły się tylko magicznymi liczbami: przesunięcie 0/5/10, długość 489/479.7/402.9, ×6).
    /// Wartości per poziom ustawia się w inspektorze; aktualizacja tylko gdy pozycja się zmieni.
    /// </summary>
    public class LevelProgressBar : MonoBehaviour {
        [SerializeField] private Transform vehicle;
        [SerializeField] private Slider slider;

        [Tooltip("Wartość slidera na starcie poziomu (A-E: 0, E-J: 5, J-O: 10)")]
        [SerializeField] private float startValue;
        [Tooltip("Pozycja X pojazdu na końcu poziomu (A-E: 489, E-J: 479.7, J-O: 402.9)")]
        [SerializeField, Min(0.01f)] private float levelLength = 489f;
        [Tooltip("O ile wzrasta wartość slidera na długości poziomu (liczba odcinków liter)")]
        [SerializeField] private float valueSpan = 6f;

        private float _lastX = float.NaN;

        private void Update() {
            if (vehicle == null) return; // pojazd wyłączony po śmierci
            var x = vehicle.position.x;
            if (Mathf.Approximately(x, _lastX)) return;
            _lastX = x;
            slider.value = startValue + x / levelLength * valueSpan;
        }
    }
}
