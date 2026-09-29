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
        [Tooltip("Po tym czasie odpadnięte koła znikają")]
        [SerializeField, Min(0f)] private float wreckLifetime = 4f;
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
        private PlayerPowerUps _powerUps;
        private float _invulnerableUntil;

        private bool IsInJumpPhase => Time.time < _jumpEndTime;

        public bool IsAutopilot => autopilot;
        public bool CanJump => !IsDead && Time.time >= _nextJumpAllowedTime;

        /// <summary>Gaz sterowany przez autopilota (-1..1), używany tylko gdy autopilot jest włączony.</summary>
        public float AutopilotThrottle { get; set; }

        public void SetAutopilot(bool value) => autopilot = value;

        /// <summary>Skok zlecony z kodu (autopilot), z pominięciem blokady wejścia gracza.</summary>
        public void RequestJump() {
            if (!IsDead) _jumpRequested = true;
        }

        private void Awake() {
            _tires = GetComponentsInChildren<VehicleTireController>();
            _powerUps = GetComponent<PlayerPowerUps>();
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
            var throttle = autopilot ? AutopilotThrottle : input == null ? 0f : input.Throttle;
            var step = playerParams.SpeedStep * dt;

            _speed = Mathf.Approximately(throttle, 0f)
                ? Mathf.MoveTowards(_speed, playerParams.PlayerSpeed, step)
                : Mathf.Clamp(_speed + throttle * step, playerParams.MinimalSpeed, MaxSpeed);
        }

        private float MaxSpeed => playerParams.MaxSpeed * (_powerUps != null ? _powerUps.SpeedMultiplier : 1f);

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
                    ? new Vector2(playerParams.JumpHorizontalSpeed, playerParams.JumpHeightAccelerate * (autopilot ? 1f : VehicleUpgrades.JumpMultiplier))
                    : new Vector2(_speed, Mathf.Min(body.linearVelocity.y, -playerParams.GroundStickSpeed));
            }
        }

        /// <summary>Wywoływane przez przeszkody i pociski. Idempotentne – wiele trafień w tej samej klatce = jedna śmierć.</summary>
        public void PlayerDeath() {
            if (IsDead || Time.time < _invulnerableUntil) return;
            if (_powerUps != null && _powerUps.TryAbsorbHit()) {
                _invulnerableUntil = Time.time + 1f;
                return;
            }

            IsDead = true;

            AudioManager.Play(SoundId.Crash);
            if (explosionEffect != null) {
                var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, explosionLifetime);
            }

            Died?.Invoke();
            Combo.Reset();
            GameEvents.RaisePlayerDied();
            BreakApart();
        }

        /// <summary>
        /// Rozpad pojazdu jak w makiecie: koła odpadają i toczą się osobno, nadwozie podskakuje i koziołkuje.
        /// </summary>
        private void BreakApart() {
            var body = GetComponent<Rigidbody2D>();
            var forward = _speed;
            foreach (var tire in _tires) {
                if (tire.TryGetComponent<WheelJoint2D>(out var joint)) Destroy(joint);
                tire.transform.SetParent(null, true);
                var rb = tire.Body;
                rb.linearVelocity = new Vector2(forward * UnityEngine.Random.Range(-0.2f, 0.9f), UnityEngine.Random.Range(3f, 6f));
                rb.angularVelocity = UnityEngine.Random.Range(-720f, 720f);
                Destroy(tire.gameObject, wreckLifetime);
            }

            if (body != null) {
                body.bodyType = RigidbodyType2D.Dynamic;
                body.constraints = RigidbodyConstraints2D.None;
                body.linearVelocity = new Vector2(forward * 0.5f, 5f);
                body.angularVelocity = (UnityEngine.Random.value < 0.5f ? -1f : 1f) * UnityEngine.Random.Range(200f, 400f);
            }
        }
    }
}
