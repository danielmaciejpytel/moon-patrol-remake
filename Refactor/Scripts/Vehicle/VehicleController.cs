using System;
using Audio;
using Core;
using GameInput;
using ScriptableObjects.Player;
using UnityEngine;

namespace Vehicle {
    /// <summary>
    /// Pojazd gracza: jedno miejsce czytania wejścia, fizyka w FixedUpdate, jednorazowa śmierć.
    /// Naprawione błędy z VehicleTireController:
    /// - "+ 100 * Time.deltaTime" w prędkości → prędkość zależna od FPS (przy 30 FPS szybciej niż przy 144);
    /// - pozycja Y podłoża (_pos) była używana jako prędkość Y;
    /// - Raycast z kierunkiem Vector2.negativeInfinity (NaN) i bez sprawdzenia null;
    /// - BackToNormalSpeed porównywał "_playerSpeed - deltaTime*step" z 0.2 zamiast z prędkością bazową;
    /// - velocity ustawiane w Update zamiast FixedUpdate.
    /// </summary>
    [DisallowMultipleComponent]
    public class VehicleController : MonoBehaviour {
        [SerializeField] private PlayerParamsSO playerParams;
        [SerializeField] private InputReaderSO input;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField, Min(0f)] private float explosionLifetime = 4f;
        [Tooltip("W menu pojazd jedzie sam ze stałą prędkością i ignoruje sterowanie")]
        [SerializeField] private bool autopilot;

        public event Action Died;

        public bool IsDead { get; private set; }
        public float CurrentSpeed => _speed;

        private VehicleTireController[] _tires = Array.Empty<VehicleTireController>();
        private float _speed;
        private float _jumpEndTime = float.NegativeInfinity;
        private float _nextJumpAllowedTime;
        private bool _jumpRequested;

        private bool IsInJumpPhase => Time.time < _jumpEndTime;

        public void SetAutopilot(bool value) => autopilot = value;

        private void Awake() {
            _tires = GetComponentsInChildren<VehicleTireController>();
            _speed = playerParams.PlayerSpeed;
        }

        private void OnEnable() {
            if (input != null) input.JumpPressed += OnJumpPressed;
        }

        private void OnDisable() {
            if (input != null) input.JumpPressed -= OnJumpPressed;
        }

        private void OnJumpPressed() {
            if (!autopilot && !IsDead) _jumpRequested = true;
        }

        private void FixedUpdate() {
            if (IsDead) return;

            UpdateSpeed(Time.fixedDeltaTime);
            TryStartJump();
            ApplyVelocity();
        }

        private void UpdateSpeed(float dt) {
            var throttle = autopilot || input == null ? 0f : input.Throttle;
            var step = playerParams.SpeedStep * dt;

            _speed = Mathf.Approximately(throttle, 0f)
                ? Mathf.MoveTowards(_speed, playerParams.PlayerSpeed, step)
                : Mathf.Clamp(_speed + throttle * step, playerParams.MinimalSpeed, playerParams.MaxSpeed);
        }

        private void TryStartJump() {
            if (!_jumpRequested) return;
            _jumpRequested = false;
            if (Time.time < _nextJumpAllowedTime) return;

            _jumpEndTime = Time.time + playerParams.JumpTimeInAir;
            // Zachowane zachowanie oryginału: kolejny skok możliwy po 2× czasie fazy wznoszenia.
            _nextJumpAllowedTime = Time.time + 2f * playerParams.JumpTimeInAir;
            AudioManager.Play(SoundId.Jump);
        }

        private void ApplyVelocity() {
            var jumping = IsInJumpPhase;
            foreach (var tire in _tires) {
                var body = tire.Body;
                body.linearVelocity = jumping
                    ? new Vector2(playerParams.JumpHorizontalSpeed, playerParams.JumpHeightAccelerate)
                    : new Vector2(_speed, Mathf.Min(body.linearVelocity.y, -playerParams.GroundStickSpeed));
            }
        }

        /// <summary>Wywoływane przez przeszkody i pociski. Idempotentne – wiele trafień w tej samej klatce = jedna śmierć.</summary>
        public void PlayerDeath() {
            if (IsDead) return;
            IsDead = true;

            AudioManager.Play(SoundId.Crash);
            if (explosionEffect != null) {
                var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, explosionLifetime);
            }

            Died?.Invoke();
            GameEvents.RaisePlayerDied();
            gameObject.SetActive(false);
        }
    }
}
