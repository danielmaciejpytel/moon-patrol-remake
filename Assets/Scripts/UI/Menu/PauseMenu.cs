using GameInput;
using Scenes;
using UI.Kit;
using UnityEngine;

namespace UI.Menu {
    /// <summary>
    /// Pauza w etapie (Esc / P / Start na padzie): zatrzymuje czas i dźwięk, blokuje sterowanie pojazdem.
    /// Resume, Settings i powrót do menu; Esc / B zamyka pauzę.
    /// </summary>
    public class PauseMenu : UiScreen {
        [SerializeField] private InputReaderSO input;
        [SerializeField] private LevelController levelController;
        [SerializeField] private MenuItemView resumeItem;
        [SerializeField] private MenuItemView settingsItem;
        [SerializeField] private MenuItemView menuItem;
        [SerializeField] private UiScreen settingsScreen;

        private bool _subscribed;

        private void Awake() {
            resumeItem.Button.onClick.AddListener(Close);
            settingsItem.Button.onClick.AddListener(() => settingsScreen.Open(() => Select(settingsItem.gameObject)));
            menuItem.Button.onClick.AddListener(() => {
                Close();
                if (levelController == null) levelController = FindFirstObjectByType<LevelController>();
                levelController.QuitToMenu();
            });
            input.PausePressed += OnPausePressed;
            _subscribed = true;
            gameObject.SetActive(false);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (_subscribed) input.PausePressed -= OnPausePressed;
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        private void OnPausePressed() {
            if (!IsOpen) {
                var vehicle = FindFirstObjectByType<Vehicle.VehicleController>();
                if (vehicle != null && vehicle.IsDead) return;
                Open();
            }
            else if (Top == this) Close();
        }

        protected override void OnOpened() {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            input.GameplayEnabled = false;
        }

        protected override void OnClosedInternal() {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            input.GameplayEnabled = true;
        }
    }
}
