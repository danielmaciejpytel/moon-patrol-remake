using System.Collections;
using Core;
using GameInput;
using ScriptableObjects.Scenes;
using ScriptableObjects.Score;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes {
    /// <summary>
    /// Przepływ scen. Zmiany:
    /// - korutyny zamiast Invoke("LoadNextLevel") / Invoke("GameOver") (stringi odporne na refaktor tylko z nameof);
    /// - reakcja na GameEvents.PlayerDied zamiast wywołań z VehicleController przez GameObject.Find;
    /// - następna scena po buildIndex + 1 z kontrolą zakresu (GetNameNextLevel rzucał wyjątek na ostatniej scenie
    ///   – Substring z indeksem -1 dla pustej ścieżki);
    /// - rola sceny jako enum (Auto = wykrycie po nazwie, zgodnie z dotychczasowym zachowaniem);
    /// - usunięte martwe pola (_gameOverScene, _currentLevel, _showStageSummary) i SetCurrentLevel();
    /// - wyjście z gry (Esc) działa we wszystkich scenach, także w menu.
    /// </summary>
    public class LevelController : MonoBehaviour {
        public enum SceneRole {
            Auto,
            Menu,
            Level,
            StageSummary,
            FinalStageSummary
        }

        private const string StageSummaryPrefix = "StageSummary";

        [SerializeField] private ScenesSO scenesScriptableObject;
        [SerializeField] private ScoreStoreSO scoreStore;
        [SerializeField] private InputReaderSO input;
        [SerializeField] private SceneRole role = SceneRole.Auto;
        [Tooltip("Nazwa sceny ostatniego podsumowania – po niej ładowany jest ekran końca gry")]
        [SerializeField] private string finalStageSummaryScene = "StageSummary J-O";
        [SerializeField, Min(0f)] private float loadNextSceneDelay = 5f;
        [SerializeField, Min(0f)] private float gameOverDelay = 3f;

        private bool _transitionStarted;

        private void OnEnable() {
            GameEvents.PlayerDied += OnPlayerDied;
            if (input != null) input.QuitPressed += QuitGame;
        }

        private void OnDisable() {
            GameEvents.PlayerDied -= OnPlayerDied;
            if (input != null) input.QuitPressed -= QuitGame;
        }

        private void Start() {
            var scene = SceneManager.GetActiveScene();
            scenesScriptableObject.CurrentScene = scene.name;
            scenesScriptableObject.CurrentSceneIndex = scene.buildIndex;

            switch (ResolveRole(scene)) {
                case SceneRole.Level:
                    scenesScriptableObject.LastPlayedLevel = scene.name;
                    scenesScriptableObject.CurrentStageLabel = scene.name;
                    break;
                case SceneRole.StageSummary:
                    StartCoroutine(LoadAfterDelay(LoadNextLevel, loadNextSceneDelay));
                    break;
                case SceneRole.FinalStageSummary:
                    StartCoroutine(LoadAfterDelay(GameFinished, loadNextSceneDelay));
                    break;
            }
        }

        /// <summary>Wywoływane z menu – nowa gra od zera.</summary>
        public void StartGame() {
            scoreStore.ResetForNewGame();
            LoadNextLevel();
        }

        /// <summary>Wywoływane przez CheckPoint na końcu etapu.</summary>
        public void LoadStageSummary() => LoadNextLevel();

        public void GameOver() => LoadScene(scenesScriptableObject.GameOverScene);

        private void OnPlayerDied() {
            if (scoreStore != null) scoreStore.RevertStage();
            StartCoroutine(LoadAfterDelay(GameOver, gameOverDelay));
        }

        private void LoadNextLevel() {
            var nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextIndex >= SceneManager.sceneCountInBuildSettings) {
                Debug.LogError($"{nameof(LevelController)}: no scene after index {nextIndex - 1} in Build Settings", this);
                return;
            }

            if (_transitionStarted) return;
            _transitionStarted = true;
            SceneManager.LoadScene(nextIndex);
        }

        private void GameFinished() => LoadScene(scenesScriptableObject.GameFinishedScene);

        private void LoadScene(string sceneName) {
            if (_transitionStarted) return;
            _transitionStarted = true;
            SceneManager.LoadScene(sceneName);
        }

        private static IEnumerator LoadAfterDelay(System.Action load, float delay) {
            yield return new WaitForSeconds(delay);
            load();
        }

        private SceneRole ResolveRole(Scene scene) {
            if (role != SceneRole.Auto) return role;
            if (scene.name == finalStageSummaryScene) return SceneRole.FinalStageSummary;
            if (scene.name.StartsWith(StageSummaryPrefix)) return SceneRole.StageSummary;
            return scene.buildIndex == 0 ? SceneRole.Menu : SceneRole.Level;
        }

        private static void QuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
