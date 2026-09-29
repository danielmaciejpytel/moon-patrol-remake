using Scenes;
using ScriptableObjects.Course;
using Settings;
using TMPro;
using UI.Kit;
using UnityEngine;
using Vehicle;

namespace UI.Menu {
    /// <summary>
    /// Menu główne z makiety: Continue, New game, Garage, High scores, Settings, Quit + karta najlepszego wyniku.
    /// Nawigacja strzałkami, padem i myszą; pojazd w tle jedzie na autopilocie.
    /// </summary>
    public class MainMenuController : UiScreen {
        [SerializeField] private LevelController levelController;
        [SerializeField] private CourseSO course;
        [SerializeField] private VehicleController menuVehicle;

        [Header("Items")]
        [SerializeField] private MenuItemView continueItem;
        [SerializeField] private MenuItemView newGameItem;
        [SerializeField] private MenuItemView garageItem;
        [SerializeField] private MenuItemView highScoresItem;
        [SerializeField] private MenuItemView settingsItem;
        [SerializeField] private MenuItemView quitItem;

        [Header("Screens")]
        [SerializeField] private UiScreen settingsScreen;
        [SerializeField] private UiScreen garageScreen;
        [SerializeField] private UiScreen rankingScreen;

        [Header("Best run card")]
        [SerializeField] private TMP_Text bestScore;
        [SerializeField] private TMP_Text bestCourse;
        [SerializeField] private TMP_Text bestCheckpoint;
        [SerializeField] private TMP_Text bestPilot;

        private void Start() {
            if (levelController == null) levelController = FindFirstObjectByType<LevelController>();
            if (menuVehicle == null) menuVehicle = FindFirstObjectByType<VehicleController>();
            if (menuVehicle != null) menuVehicle.SetAutopilot(true);
            var weapon = menuVehicle != null ? menuVehicle.GetComponentInChildren<VehicleWeaponController>() : null;
            if (weapon != null) weapon.SetShootingEnabled(false);

            continueItem.Button.onClick.AddListener(OnContinue);
            newGameItem.Button.onClick.AddListener(() => levelController.StartGame());
            garageItem.Button.onClick.AddListener(() => OpenChild(garageScreen, garageItem));
            highScoresItem.Button.onClick.AddListener(() => OpenChild(rankingScreen, highScoresItem));
            settingsItem.Button.onClick.AddListener(() => OpenChild(settingsScreen, settingsItem));
            quitItem.Button.onClick.AddListener(LevelController.QuitGame);

            garageItem.Interactable = garageScreen != null;
            highScoresItem.Interactable = rankingScreen != null;

            RefreshContinue();
            RefreshBestRun();
            FirstSelected = continueItem.Interactable ? continueItem.gameObject : newGameItem.gameObject;
            Open();
        }

        protected override void OnCancel() { }

        private void OpenChild(UiScreen screen, MenuItemView returnTo) {
            if (screen == null) return;
            screen.Open(() => {
                RefreshBestRun();
                Select(returnTo.gameObject);
            });
        }

        private void RefreshContinue() {
            var progress = GameProgress.Data;
            var stage = course != null ? course.Get(progress.stageIndex) : null;
            continueItem.Interactable = progress.hasRun && stage != null;
            continueItem.SetDetail(continueItem.Interactable ? $"STAGE {stage.Label} · {progress.score:0000000}" : "NO SAVED RUN");
            newGameItem.SetDetail("COURSE A–Z");
        }

        private void RefreshBestRun() {
            var best = GameProgress.Best;
            if (bestScore != null) bestScore.text = (best?.score ?? 0).ToString("0000000");
            if (bestCourse != null) bestCourse.text = "A–Z";
            if (bestCheckpoint != null) bestCheckpoint.text = best != null ? CourseSO.Letter(best.reached).ToString() : "–";
            if (bestPilot != null) bestPilot.text = best != null ? best.pilot : "–";
        }

        private void OnContinue() {
            var progress = GameProgress.Data;
            var stage = course.Get(progress.stageIndex);
            if (stage == null) return;
            levelController.ContinueGame(stage.scene, progress.score);
        }
    }
}
