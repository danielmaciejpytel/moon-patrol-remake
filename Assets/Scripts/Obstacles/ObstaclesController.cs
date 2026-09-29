using UnityEngine;
using Vehicle;

namespace Obstacles {
    /// <summary>
    /// Mina / dziura: zabija pojazd, gdy wjedzie w nią koło.
    /// Zmiany: jedno TryGetComponent zamiast dwóch GetComponent; usunięty Debug.Log z błędnym "$" w interpolacji.
    /// </summary>
    public class ObstaclesController : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            // Tylko koła – nadwozie przelatujące nad przeszkodą w skoku jej nie aktywuje (jak w oryginale).
            if (!other.TryGetComponent(out VehicleTireController tire) || tire.Vehicle == null) return;
            tire.Vehicle.PlayerDeath();
        }
    }
}
