using ScriptableObjects.Course;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    /// <summary>
    /// Jeden pasek postępu zamiast trzech prawie identycznych klas LevelAEProgressBar / LevelEJProgressBar /
    /// LevelJOProgress (różniły się tylko magicznymi liczbami: przesunięcie 0/5/10, długość 489/479.7/402.9, ×6).
    /// Gdy przypisany jest CourseSO, zakres liter i długość etapu są brane z trasy A–Z (slider 0..25).
    /// Aktualizacja tylko gdy pozycja się zmieni.
    /// </summary>
    public class LevelProgressBar : MonoBehaviour {
        [SerializeField] private Transform vehicle;
        [SerializeField] private Slider slider;
        [SerializeField] private CourseSO course;

        [Tooltip("Bez CourseSO: wartość slidera na starcie poziomu (A-E: 0, E-J: 5, J-O: 10)")]
        [SerializeField] private float startValue;
        [Tooltip("Bez CourseSO: pozycja X pojazdu na końcu poziomu (A-E: 489, E-J: 479.7, J-O: 402.9)")]
        [SerializeField, Min(0.01f)] private float levelLength = 489f;
        [Tooltip("Bez CourseSO: o ile wzrasta wartość slidera na długości poziomu")]
        [SerializeField] private float valueSpan = 6f;

        private float _startX;
        private float _lastX = float.NaN;

        private void Start() {
            if (vehicle == null) {
                var v = FindFirstObjectByType<Vehicle.VehicleController>();
                if (v != null) vehicle = v.transform;
            }

            var stage = course != null ? course.Get(course.IndexOfScene(SceneManager.GetActiveScene().name)) : null;
            if (stage == null) return;
            slider.minValue = 0f;
            slider.maxValue = CourseSO.LastLetter;
            startValue = stage.fromLetter;
            valueSpan = stage.toLetter - stage.fromLetter;
            _startX = stage.startX;
            levelLength = Mathf.Max(0.01f, stage.endX - stage.startX);
        }

        private void Update() {
            if (vehicle == null) return; // pojazd wyłączony po śmierci
            var x = vehicle.position.x;
            if (Mathf.Approximately(x, _lastX)) return;
            _lastX = x;
            slider.value = startValue + Mathf.Clamp01((x - _startX) / levelLength) * valueSpan;
        }
    }
}
