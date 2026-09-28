using UnityEngine;

namespace Vehicle {
    /// <summary>
    /// Koło pojazdu – teraz komponent pasywny. Wcześniej każde z 3 kół osobno czytało klawiaturę,
    /// liczyło prędkość, robiło FindObjectOfType&lt;AudioManager&gt;() i odtwarzało dźwięk skoku (3× na skok).
    /// Całą logiką ruchu zarządza VehicleController; przeszkody nadal rozpoznają koło po tym komponencie.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public class VehicleTireController : MonoBehaviour {
        [SerializeField] private Rigidbody2D tireRigidbody2D;

        public Rigidbody2D Body => tireRigidbody2D;
        public VehicleController Vehicle { get; private set; }

        private void Awake() {
            if (tireRigidbody2D == null) tireRigidbody2D = GetComponent<Rigidbody2D>();
            Vehicle = GetComponentInParent<VehicleController>();
        }

        private void Reset() => tireRigidbody2D = GetComponent<Rigidbody2D>();
    }
}
