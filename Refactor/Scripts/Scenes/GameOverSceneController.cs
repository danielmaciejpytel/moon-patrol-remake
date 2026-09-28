using GameInput;
using ScriptableObjects.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes {
    /// <summary>
    /// Ekran "continue?" po śmierci. Zmiany: TextMeshPro, Input System (AnyKey) zamiast Input.anyKeyDown,
    /// krótka blokada wejścia (klawisz wciśnięty w chwili śmierci nie pomija ekranu),
    /// powrót do menu po nazwie sceny zamiast LoadScene(0), usunięte nieużywane pole _levelController,
    /// tekst aktualizowany tylko przy zmianie sekundy.
    /// </summary>
    public class GameOverSceneController : MonoBehaviour {
        [SerializeField] private ScenesSO scenesSO;
        [SerializeField] private InputReaderSO input;
        [SerializeField] private TMP_Text timer;
        [SerializeField, Min(1f)] private float countdownSeconds = 10f;
        [SerializeField, Min(0f)] private float inputDelay = 0.5f;

        private float _remaining;
        private int _shownSeconds = -1;
        private bool _leaving;

        private void OnEnable() => input.AnyKeyPressed += Continue;

        private void OnDisable() => input.AnyKeyPressed -= Continue;

        private void Start() => _remaining = countdownSeconds;

        private void Update() {
            if (_leaving) return;

            _remaining -= Time.deltaTime;
            var seconds = Mathf.Max(0, Mathf.CeilToInt(_remaining));
            if (seconds != _shownSeconds) {
                _shownSeconds = seconds;
                timer.SetText("{0:00}", seconds);
            }

            if (_remaining <= 0f) Leave(scenesSO.MenuScene);
        }

        private void Continue() {
            if (Time.timeSinceLevelLoad < inputDelay || string.IsNullOrEmpty(scenesSO.LastPlayedLevel)) return;
            Leave(scenesSO.LastPlayedLevel);
        }

        private void Leave(string sceneName) {
            if (_leaving) return;
            _leaving = true;
            SceneManager.LoadScene(sceneName);
        }
    }
}
