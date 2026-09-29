using Core;
using ScriptableObjects.Obstacles;
using UnityEngine;

namespace Obstacles {
    /// <summary>
    /// Punkty za przeskoczenie przeszkody. Naprawione: flaga _isColliderIsUntouched była ustawiana, ale nigdy
    /// sprawdzana → punkty mogły być przyznane wielokrotnie. Brak GameObject.Find("ScoreManager").
    /// </summary>
    public class ScoreJumpOverController : MonoBehaviour {
        [SerializeField] private ObstaclesParamsSO obstacleParamsScriptableObject;

        private bool _awarded;

        private void OnTriggerEnter2D(Collider2D other) {
            if (_awarded || !VehicleHitDetection.TryGetVehicle(other, out var vehicle) || vehicle.IsDead) return;
            _awarded = true;
            GameEvents.AwardScore(obstacleParamsScriptableObject.JumpOverScore);
        }
    }
}
