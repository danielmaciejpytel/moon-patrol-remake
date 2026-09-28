using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameInput {
    /// <summary>
    /// Jedno źródło wejścia oparte o Input System. Zastępuje KeyboardActionKeyCodeSO + Input.GetKey
    /// czytane osobno w 3× VehicleTireController, VehicleWeaponController, MenuStart i GameOverSceneController.
    /// Akcje podpina się w inspektorze (InputActionReference) z MoonPatrolControls.inputactions.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Input/InputReader")]
    public class InputReaderSO : ScriptableObject {
        [Tooltip("Oś 1D: -1 hamowanie, +1 przyspieszanie")]
        [SerializeField] private InputActionReference throttle;
        [SerializeField] private InputActionReference jump;
        [SerializeField] private InputActionReference shoot;
        [SerializeField] private InputActionReference start;
        [SerializeField] private InputActionReference quit;
        [Tooltip("Dowolny klawisz – ekran Game Over")]
        [SerializeField] private InputActionReference anyKey;

        public event Action JumpPressed;
        public event Action ShootPressed;
        public event Action StartPressed;
        public event Action QuitPressed;
        public event Action AnyKeyPressed;

        public float Throttle => IsReady(throttle) ? throttle.action.ReadValue<float>() : 0f;

        private void OnEnable() {
            Bind(jump, OnJump);
            Bind(shoot, OnShoot);
            Bind(start, OnStart);
            Bind(quit, OnQuit);
            Bind(anyKey, OnAnyKey);
            if (throttle != null) throttle.action.Enable();
        }

        private void OnDisable() {
            Unbind(jump, OnJump);
            Unbind(shoot, OnShoot);
            Unbind(start, OnStart);
            Unbind(quit, OnQuit);
            Unbind(anyKey, OnAnyKey);
            if (throttle != null) throttle.action.Disable();
        }

        private void OnJump(InputAction.CallbackContext _) => JumpPressed?.Invoke();
        private void OnShoot(InputAction.CallbackContext _) => ShootPressed?.Invoke();
        private void OnStart(InputAction.CallbackContext _) => StartPressed?.Invoke();
        private void OnQuit(InputAction.CallbackContext _) => QuitPressed?.Invoke();
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
