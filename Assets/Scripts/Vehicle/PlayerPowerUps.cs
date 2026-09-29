using System;
using Core;
using UnityEngine;

namespace Vehicle {
    public enum PowerUpKind {
        TwinShot,
        Shield,
        Turbo
    }

    /// <summary>
    /// Power-upy wypadające z zestrzelonych UFO: podwójny strzał, tarcza (pochłania jedno trafienie), turbo (+15% prędkości).
    /// Tarcza z garażu (Shield dome) daje ładunki na cały etap.
    /// </summary>
    public class PlayerPowerUps : MonoBehaviour {
        [SerializeField, Min(1f)] private float duration = 10f;
        [SerializeField, Min(0f)] private float turboBonus = 0.15f;

        private PowerUpKind? _active;
        private float _endTime;
        private int _shieldCharges;

        public event Action Changed;

        public PowerUpKind? Active => _active != null && Time.time < _endTime ? _active : null;
        public float Remaining01 => Active == null ? 0f : Mathf.Clamp01((_endTime - Time.time) / duration);
        public int ShieldCharges => _shieldCharges;

        public bool TwinShot => Active == PowerUpKind.TwinShot || VehicleUpgrades.TwinShot;
        public float SpeedMultiplier => (Active == PowerUpKind.Turbo ? 1f + turboBonus : 1f) * VehicleUpgrades.MaxSpeedMultiplier;

        private void Start() {
            _shieldCharges = VehicleUpgrades.ShieldCharges;
            Changed?.Invoke();
        }

        private void Update() {
            if (_active != null && Time.time >= _endTime) {
                _active = null;
                Changed?.Invoke();
            }
        }

        public void Activate(PowerUpKind kind) {
            if (kind == PowerUpKind.Shield) _shieldCharges++;
            else {
                _active = kind;
                _endTime = Time.time + duration;
            }

            GameEvents.ShowToast(Name(kind), kind == PowerUpKind.Shield ? "ABSORBS ONE HIT" : $"{duration:0} SECONDS", 1.4f);
            Changed?.Invoke();
        }

        /// <summary>Zwraca true, jeśli tarcza pochłonęła trafienie.</summary>
        public bool TryAbsorbHit() {
            if (_shieldCharges <= 0) return false;
            _shieldCharges--;
            GameEvents.ShowToast("SHIELD", _shieldCharges > 0 ? $"{_shieldCharges} LEFT" : "DEPLETED", 1.2f);
            Changed?.Invoke();
            return true;
        }

        public static string Name(PowerUpKind kind) => kind switch {
            PowerUpKind.TwinShot => "TWIN SHOT",
            PowerUpKind.Shield => "SHIELD",
            _ => "TURBO"
        };
    }
}
