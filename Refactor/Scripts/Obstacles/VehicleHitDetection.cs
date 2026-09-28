using UnityEngine;
using Vehicle;

namespace Obstacles {
    /// <summary>
    /// Wspólne rozpoznawanie pojazdu w triggerach (wcześniej każda przeszkoda robiła to inaczej,
    /// np. StoneController wywoływał PlayerDeath() na null, gdy trafiło samo koło).
    /// </summary>
    public static class VehicleHitDetection {
        public static bool TryGetVehicle(Collider2D other, out VehicleController vehicle) {
            if (other.TryGetComponent(out VehicleTireController tire) && tire.Vehicle != null) {
                vehicle = tire.Vehicle;
                return true;
            }

            vehicle = other.GetComponentInParent<VehicleController>();
            return vehicle != null;
        }
    }
}
