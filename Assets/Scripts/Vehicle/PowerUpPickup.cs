using Obstacles;
using UnityEngine;

namespace Vehicle {
    /// <summary>Power-up wypadający z zestrzelonego UFO: spada na drogę i działa po zebraniu przez pojazd.</summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PowerUpPickup : MonoBehaviour {
        [SerializeField] private PowerUpKind kind;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color twinShotColor = new Color(0.94f, 0.63f, 0.23f);
        [SerializeField] private Color shieldColor = new Color(0.26f, 0.82f, 0.75f);
        [SerializeField] private Color turboColor = new Color(0.91f, 0.15f, 0.37f);
        [SerializeField, Min(1f)] private float lifetime = 8f;

        private bool _taken;

        public static PowerUpPickup Spawn(PowerUpPickup prefab, Vector3 position, float forwardSpeed) {
            var pickup = Instantiate(prefab, position, Quaternion.identity);
            pickup.SetKind((PowerUpKind)Random.Range(0, 3));
            pickup.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(forwardSpeed, 1.5f);
            return pickup;
        }

        public void SetKind(PowerUpKind value) {
            kind = value;
            if (spriteRenderer != null)
                spriteRenderer.color = kind switch {
                    PowerUpKind.TwinShot => twinShotColor,
                    PowerUpKind.Shield => shieldColor,
                    _ => turboColor
                };
        }

        private void Start() => Destroy(gameObject, lifetime);

        private void Update() => transform.Rotate(0f, 0f, 180f * Time.deltaTime);

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag(Core.GameTags.Ground)) {
                // Leży na drodze – pojazd zbiera go przejeżdżając.
                var body = GetComponent<Rigidbody2D>();
                body.linearVelocity = Vector2.zero;
                body.bodyType = RigidbodyType2D.Kinematic;
                return;
            }

            if (_taken || !VehicleHitDetection.TryGetVehicle(other, out var vehicle) || vehicle.IsDead) return;
            if (!vehicle.TryGetComponent<PlayerPowerUps>(out var powerUps)) return;
            _taken = true;
            powerUps.Activate(kind);
            Audio.AudioManager.Play(Audio.SoundId.Checkpoint);
            Destroy(gameObject);
        }
    }
}
