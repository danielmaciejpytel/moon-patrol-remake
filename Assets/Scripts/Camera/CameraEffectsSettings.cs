using Settings;
using UnityEngine;

namespace Camera {
    /// <summary>
    /// Włącza i wyłącza efekty ekranu kamery według ustawień gracza (linie CRT, przesunięcie kolorów)
    /// i obsługuje wstrząs ekranu po śmierci pojazdu.
    /// Wstrząs jest dodawany po CinemachineBrain (późna kolejność wykonania), więc nie walczy z podążaniem kamery.
    /// </summary>
    [DefaultExecutionOrder(1000)]
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraEffectsSettings : MonoBehaviour {
        [SerializeField] private ShaderEffect_CRT crt;
        [SerializeField] private ShaderEffectBleedingColors bleeding;
        [SerializeField, Min(0f)] private float shakeDuration = 0.4f;
        [SerializeField, Min(0f)] private float shakeStrength = 0.25f;

        private float _shakeLeft;

        private void Awake() {
            if (crt == null) crt = GetComponent<ShaderEffect_CRT>();
            if (bleeding == null) bleeding = GetComponent<ShaderEffectBleedingColors>();
        }

        private void OnEnable() {
            GameSettings.Changed += Apply;
            Core.GameEvents.PlayerDied += Shake;
            Apply();
        }

        private void OnDisable() {
            GameSettings.Changed -= Apply;
            Core.GameEvents.PlayerDied -= Shake;
        }

        private void Apply() {
            var s = GameSettings.Data;
            if (crt != null) crt.enabled = s.crtScanlines;
            if (bleeding != null) bleeding.enabled = s.colorBleeding;
        }

        public void Shake() {
            if (GameSettings.Data.screenShake) _shakeLeft = shakeDuration;
        }

        private void LateUpdate() {
            if (_shakeLeft <= 0f) return;
            _shakeLeft -= Time.unscaledDeltaTime;
            var strength = shakeStrength * Mathf.Clamp01(_shakeLeft / shakeDuration);
            transform.position += (Vector3)(Random.insideUnitCircle * strength);
        }
    }
}
