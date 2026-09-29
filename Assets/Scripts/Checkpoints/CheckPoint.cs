using Obstacles;
using Scenes;
using UnityEngine;

namespace Checkpoints {
    /// <summary>Koniec etapu → podsumowanie. LevelController wstrzyknięty w inspektorze zamiast GameObject.Find.</summary>
    public class CheckPoint : MonoBehaviour {
        [SerializeField] private LevelController levelController;

        private bool _reached;

        private void OnTriggerEnter2D(Collider2D other) {
            if (_reached || !VehicleHitDetection.TryGetVehicle(other, out var vehicle) || vehicle.IsDead) return;
            _reached = true;

            if (levelController == null) levelController = FindFirstObjectByType<LevelController>();
            levelController.LoadStageSummary();
        }
    }
}
