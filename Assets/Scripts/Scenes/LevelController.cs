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
        [Tooltip("Czas od śmierci do powrotu na punkt kontrolny (rozpad pojazdu)")]
        [SerializeField, Min(0f)] private float respawnDelay = 2.4f;
        [Tooltip("Czas od utraty ostatniego życia do Continue")]
        [SerializeField, Min(0f)] private float gameOverDelay = 3.4f;

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
                case SceneRole.FinalStageSummary:
                    // Ekran końca gry (napisy) – potem menu z wpisem do rankingu.
                    StartCoroutine(LoadAfterDelay(LoadMenu, loadNextSceneDelay * 2f));
                    break;
                case SceneRole.StageSummary:
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

            GameRules.BombCraters = stage != null && stage.bombCraters;
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

        /// <summary>Wyjście z etapu do menu (pauza): przejazd kończy się na ostatnim punkcie kontrolnym.</summary>
        public void QuitToMenu() {
            var stage = course != null ? course.Get(scenesScriptableObject.CurrentStageIndex) : null;
            if (stage != null) EndRun(stage.fromLetter, false);
            LoadMenu();
        }

        private void EndRun(int reachedLetter, bool finished) {
            var stage = course != null ? course.Get(scenesScriptableObject.CurrentStageIndex) : null;
            var score = scoreStore.PlayerOverallScore;
            if (finished) GameProgress.ClearRun();
            if (GameProgress.RankFor(score) < 0) return;
            scenesScriptableObject.PendingRank = new RankEntry {
                score = score,
                reached = reachedLetter,
                time = scenesScriptableObject.RunTime + (finished ? 0 : scoreStore.StageTime),
                stage = stage != null ? stage.Label : ""
            };
        }

        /// <summary>
        /// 3 życia na etap: śmierć cofa do punktu kontrolnego (litera główna na początku etapu) z wynikiem z tej chwili.
        /// Po utracie wszystkich żyć – Continue: etap od początku, wynik od zera, pełne życia.
        /// </summary>
        private void OnPlayerDied() {
            if (_role != SceneRole.Level || _transitionStarted) return;
            var lastLife = scoreStore.Lives <= 1;
            var livesLeft = scoreStore.Lives - 1;
            if (lastLife) GameEvents.ShowToast("GAME OVER", "CONTINUE: STAGE RESTARTS, SCORE RESETS", 2.2f);
            else GameEvents.ShowToast("ROVER DOWN", livesLeft == 1 ? "1 LIFE LEFT" : $"{livesLeft} LIVES LEFT", 2.2f);
            StartCoroutine(LoadAfterDelay(Respawn, lastLife ? gameOverDelay : respawnDelay));
        }

        private void Respawn() {
            var gameOver = scoreStore.LoseLife();
            scenesScriptableObject.Retrying = true;
            scenesScriptableObject.ContinuedAfterGameOver = gameOver;
            if (gameOver) {
                scenesScriptableObject.RunTime = 0;
                GameProgress.SetCheckpoint(scenesScriptableObject.CurrentStageIndex, 0, 0);
            }

            LoadScene(SceneManager.GetActiveScene().name);
        }

        private void LoadAfterSummary() {
            var index = course != null ? course.IndexOfSummary(SceneManager.GetActiveScene().name) : -1;
            var next = course != null ? course.Get(index + 1) : null;
            if (index >= 0 && next == null) {
                // Koniec trasy A–Z: wynik do rankingu, powrót do menu.
                EndRun(course.Get(index).toLetter, true);
                LoadScene(scenesScriptableObject.GameFinishedScene);
                return;
            }

            if (next != null) LoadScene(next.scene);
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
            if (scene.name == scenesScriptableObject.GameFinishedScene) return SceneRole.FinalStageSummary;
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
