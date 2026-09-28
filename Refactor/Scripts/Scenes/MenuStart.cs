using GameInput;
using UnityEngine;
using Vehicle;

namespace Scenes {
    /// <summary>
    /// Menu startowe. Zmiany: InputReaderSO (Start = S / Enter / Start na padzie) zamiast Input.GetKeyDown w Update;
    /// referencje z inspektora zamiast GameObject.Find("LevelController") i GameObject.Find("Weapon");
    /// pojazd w tle jedzie na autopilocie (wcześniej reagował na strzałki w menu).
    /// </summary>
    public class MenuStart : MonoBehaviour {
        [SerializeField] private InputReaderSO input;
        [SerializeField] private LevelController levelController;
        [SerializeField] private VehicleController menuVehicle;
        [SerializeField] private VehicleWeaponController menuVehicleWeapon;

        private void Start() {
            if (levelController == null) levelController = FindFirstObjectByType<LevelController>();
            if (menuVehicle == null) menuVehicle = FindFirstObjectByType<VehicleController>();
            if (menuVehicleWeapon == null) menuVehicleWeapon = FindFirstObjectByType<VehicleWeaponController>();

            if (menuVehicle != null) menuVehicle.SetAutopilot(true);
            if (menuVehicleWeapon != null) menuVehicleWeapon.SetShootingEnabled(false);
        }

        private void OnEnable() => input.StartPressed += OnStartPressed;

        private void OnDisable() => input.StartPressed -= OnStartPressed;

        private void OnStartPressed() => levelController.StartGame();
    }
}
