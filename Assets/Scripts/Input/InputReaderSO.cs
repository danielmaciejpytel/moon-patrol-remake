using System;
using Settings;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace GameInput {
    /// <summary>
    /// Jedno źródło wejścia oparte o Input System. Zastępuje KeyboardActionKeyCodeSO + Input.GetKey
    /// czytane osobno w 3× VehicleTireController, VehicleWeaponController, MenuStart i GameOverSceneController.
    /// Akcje podpina się w inspektorze (InputActionReference) z MoonPatrolControls.inputactions.
    /// Zmienione klawisze (rebinding w ustawieniach) są wczytywane z GameSettings.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Input/InputReader")]
    public class InputReaderSO : ScriptableObject {
        [Tooltip("Oś 1D: -1 hamowanie, +1 przyspieszanie")]
        [SerializeField] private InputActionReference throttle;
        [SerializeField] private InputActionReference jump;
        [SerializeField] private InputActionReference shoot;
        [SerializeField] private InputActionReference start;
        [FormerlySerializedAs("quit")]
        [SerializeField] private InputActionReference pause;
        [Tooltip("Dowolny klawisz – ekran Game Over")]
        [SerializeField] private InputActionReference anyKey;

        public event Action JumpPressed;
        public event Action ShootPressed;
        public event Action StartPressed;
        public event Action PausePressed;
        public event Action AnyKeyPressed;

        /// <summary>Wyłączone w pauzie i menu – skok i strzał nie trafiają wtedy do pojazdu.</summary>
        public bool GameplayEnabled { get; set; } = true;

        public float Throttle => GameplayEnabled && IsReady(throttle) ? throttle.action.ReadValue<float>() : 0f;

        public InputAction ThrottleAction => throttle != null ? throttle.action : null;
        public InputAction JumpAction => jump != null ? jump.action : null;
        public InputAction ShootAction => shoot != null ? shoot.action : null;
        public InputAction PauseAction => pause != null ? pause.action : null;
        public InputActionAsset Asset => jump != null && jump.action != null ? jump.action.actionMap?.asset : null;

        private void OnEnable() {
            GameplayEnabled = true;
            LoadBindingOverrides();
            Bind(jump, OnJump);
            Bind(shoot, OnShoot);
            Bind(start, OnStart);
            Bind(pause, OnPause);
            Bind(anyKey, OnAnyKey);
            if (throttle != null) throttle.action.Enable();
        }

        private void OnDisable() {
            Unbind(jump, OnJump);
            Unbind(shoot, OnShoot);
            Unbind(start, OnStart);
            Unbind(pause, OnPause);
            Unbind(anyKey, OnAnyKey);
            if (throttle != null) throttle.action.Disable();
        }

        public void LoadBindingOverrides() {
            var asset = Asset;
            if (asset == null) return;
            asset.RemoveAllBindingOverrides();
            var json = GameSettings.Data.bindingOverrides;
            if (!string.IsNullOrEmpty(json)) asset.LoadBindingOverridesFromJson(json);
        }

        public void SaveBindingOverrides() {
            var asset = Asset;
            if (asset == null) return;
            GameSettings.Data.bindingOverrides = asset.SaveBindingOverridesAsJson();
            GameSettings.Apply();
        }

        private void OnJump(InputAction.CallbackContext _) {
            if (GameplayEnabled) JumpPressed?.Invoke();
        }

        private void OnShoot(InputAction.CallbackContext _) {
            if (GameplayEnabled) ShootPressed?.Invoke();
        }

        private void OnStart(InputAction.CallbackContext _) => StartPressed?.Invoke();
        private void OnPause(InputAction.CallbackContext _) => PausePressed?.Invoke();
        private void OnAnyKey(InputAction.CallbackContext _) => AnyKeyPressed?.Invoke();

        private static bool IsReady(InputActionReference reference) =>
            reference != null && reference.action != null && reference.action.enabled;

        private static void Bind(InputActionReference reference, Action<InputAction.CallbackContext> handler) {
            if (reference == null || reference.action == null) return;
            reference.action.performed += handler;
            reference.action.Enable();
        }

        private static void Unbind(InputActionReference reference, Action<InputAction.CallbackContext> handler) {
            if (reference == null || reference.action == null) return;
            reference.action.performed -= handler;
            reference.action.Disable();
        }
    }
}
