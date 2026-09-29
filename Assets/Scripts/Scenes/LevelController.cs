using System.Collections;
using Core;
using GameInput;
using ScriptableObjects.Course;
using ScriptableObjects.Scenes;
using ScriptableObjects.Score;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes {
    /// <summary>
    /// Przepływ scen. Zmiany:
    /// - korutyny zamiast Invoke("LoadNextLevel") / Invoke("GameOver") (stringi odporne na refaktor tylko z nameof);
    /// - reakcja na GameEvents.PlayerDied zamiast wywołań z VehicleController przez GameObject.Find;
    /// - kolejność etapów z CourseSO (trasa A–Z) zamiast buildIndex + 1 i wycinania nazwy ze ścieżki sceny;
    /// - rola sceny jako enum (Auto = wykrycie po nazwie, zgodnie z dotychczasowym zachowaniem);
    /// - punkt kontrolny na początku etapu zapisywany do GameProgress (Continue w menu), bonus za czas;
    /// - wyjście z gry z menu głównego (QUIT), Esc w poziomie otwiera pauzę.
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
        [SerializeField] private CourseSO course;
        [SerializeField] private InputReaderSO input;
        [SerializeField] private SceneRole role = SceneRole.Auto;
        [SerializeField, Min(0f)] private float loadNextSceneDelay = 5f;
        [SerializeField, Min(0f)] private float gameOverDelay = 3f;

        private bool _transitionStarted;
        private SceneRole _role;

        public float NextSceneDelay => loadNextSceneDelay;
        public CourseSO Course => course;

        private void OnEnable() => GameEvents.PlayerDied += OnPlayerDied;

        private void OnDisable() => GameEvents.PlayerDied -= OnPlayerDied;

        private void Start() {
            var scene = SceneManager.GetActiveScene();
            scenesScriptableObject.CurrentScene = scene.name;
            scenesScriptableObject.CurrentSceneIndex = scene.buildIndex;
            _role = ResolveRole(scene);

            switch (_role) {
                case SceneRole.Menu:
                    scenesScriptableObject.CurrentStageIndex = -1;
                    if (input != null) input.GameplayEnabled = true;
                    break;
                case SceneRole.Level:
                    EnterStage(scene.name);
                    break;
                case SceneRole.StageSummary:
                case SceneRole.FinalStageSummary:
                    StartCoroutine(LoadAfterDelay(LoadAfterSummary, loadNextSceneDelay));
                    break;
            }
        }

        private void EnterStage(string sceneName) {
            var index = course != null ? course.IndexOfScene(sceneName) : -1;
            var stage = course != null ? course.Get(index) : null;
            scenesScriptableObject.LastPlayedLevel = sceneName;
            scenesScriptableObject.CurrentStageIndex = index;
            scenesScriptableObject.CurrentStageLabel = stage != null ? stage.Label : sceneName;
            if (input != null) input.GameplayEnabled = true;

            var retry = scenesScriptableObject.Retrying;
            scenesScriptableObject.Retrying = false;
            if (stage == null) return;

            var letter = CourseSO.Letter(stage.fromLetter);
            if (!retry) {
                // Nowy punkt kontrolny na literze głównej: od niego wznawia Continue i po nim wracamy po śmierci.
                scoreStore.BeginStage();
                GameProgress.SetCheckpoint(index, scoreStore.PlayerOverallScore, scenesScriptableObject.RunTime);
                GameProgress.ReachLetter(stage.fromLetter);
                GameEvents.ShowToast($"POINT {letter}", $"STAGE {stage.Label} · {stage.info}", 2.6f);
            }
            else if (scenesScriptableObject.ContinuedAfterGameOver) {
                scenesScriptableObject.ContinuedAfterGameOver = false;
                GameEvents.ShowToast("CONTINUE", $"STAGE {stage.Label} · {ScoreStoreSO.MaxLives} LIVES", 2.2f);
            }
            else {
                GameEvents.ShowToast($"POINT {letter}", "BACK TO CHECKPOINT", 1.7f);
            }
        }

        /// <summary>Wywoływane z menu – nowa gra od zera.</summary>
        public void StartGame() {
            scoreStore.ResetForNewGame();
            scenesScriptableObject.RunTime = 0;
            scenesScriptableObject.Retrying = false;
            var first = course != null ? course.Get(0) : null;
            if (first != null) LoadScene(first.scene);
            else LoadNextBuildIndex();
        }

        /// <summary>Continue z menu: etap z ostatniego punktu kontrolnego i zapisany wynik.</summary>
        public void ContinueGame(string sceneName, int score) {
            scoreStore.ResumeGame(score);
            scenesScriptableObject.RunTime = GameProgress.Data.runTime;
            scenesScriptableObject.Retrying = false;
            LoadScene(sceneName);
        }

        /// <summary>Wywoływane przez CheckPoint na końcu etapu.</summary>
        public void LoadStageSummary() {
            if (_transitionStarted) return;
            var index = scenesScriptableObject.CurrentStageIndex;
            var stage = course != null ? course.Get(index) : null;
            if (stage == null) {
                LoadNextBuildIndex();
                return;
            }

            scenesScriptableObject.RunTime += scoreStore.StageTime;
            scoreStore.AddBonus(scoreStore.StageTime <= stage.averageTime ? stage.goodBonus : 0);
            LoadScene(stage.summaryScene);
        }

        public void GameOver() => LoadScene(scenesScriptableObject.GameOverScene);

        public void LoadMenu() => LoadScene(scenesScriptableObject.MenuScene);

        private void OnPlayerDied() {
            if (scoreStore != null) scoreStore.RevertStage();
            StartCoroutine(LoadAfterDelay(GameOver, gameOverDelay));
        }

        private void LoadAfterSummary() {
            var index = course != null ? course.IndexOfSummary(SceneManager.GetActiveScene().name) : -1;
            var next = course != null ? course.Get(index + 1) : null;
            if (index >= 0 && next == null) {
                LoadScene(scenesScriptableObject.GameFinishedScene);
                return;
            }

            if (next != null) LoadScene(next.scene);
            else if (_role == SceneRole.FinalStageSummary) LoadScene(scenesScriptableObject.GameFinishedScene);
            else LoadNextBuildIndex();
        }

        private void LoadNextBuildIndex() {
            var nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextIndex >= SceneManager.sceneCountInBuildSettings) {
                Debug.LogError($"{nameof(LevelController)}: no scene after index {nextIndex - 1} in Build Settings", this);
                return;
            }

            if (_transitionStarted) return;
            _transitionStarted = true;
            SceneManager.LoadScene(nextIndex);
        }

        private void LoadScene(string sceneName) {
            if (_transitionStarted) return;
            _transitionStarted = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        private static IEnumerator LoadAfterDelay(System.Action load, float delay) {
            yield return new WaitForSeconds(delay);
            load();
        }

        private SceneRole ResolveRole(Scene scene) {
            if (role != SceneRole.Auto) return role;
            if (scene.name.StartsWith(StageSummaryPrefix)) return SceneRole.StageSummary;
            if (course != null && course.IndexOfScene(scene.name) >= 0) return SceneRole.Level;
            return scene.buildIndex == 0 ? SceneRole.Menu : SceneRole.Level;
        }

        public static void QuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
