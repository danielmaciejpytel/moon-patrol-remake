using System.Collections;
using System.Collections.Generic;
using Core;
using ScriptableObjects.Score;
using Settings;
using TMPro;
using UI.Kit;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    /// <summary>
    /// HUD z makiety: najlepszy wynik, litery trasy A–Z nad paskiem postępu, życia i komunikaty (toast).
    /// Wynik, czas i wypełnienie paska obsługują ScoreUIController, StageTime i LevelProgressBar.
    /// </summary>
    public class HudController : MonoBehaviour {
        [SerializeField] private ScoreStoreSO scoreStore;
        [SerializeField] private TMP_Text hiScore;

        [Header("Course")]
        [SerializeField] private Slider courseBar;
        [Tooltip("Litery trasy nad paskiem – nazwa obiektu = indeks litery (0 = A, 25 = Z)")]
        [SerializeField] private List<TMP_Text> courseLetters = new List<TMP_Text>();

        [Header("Lives")]
        [SerializeField] private List<Graphic> lives = new List<Graphic>();

        [Header("Toast")]
        [SerializeField] private CanvasGroup toast;
        [SerializeField] private TMP_Text toastTitle;
        [SerializeField] private TMP_Text toastSubtitle;

        [Header("Power-up & combo")]
        [SerializeField] private TMP_Text powerUpLabel;
        [SerializeField] private List<Graphic> powerUpPips = new List<Graphic>();
        [SerializeField] private TMP_Text combo;

        private Vehicle.PlayerPowerUps _powerUps;
        private Coroutine _toastRoutine;
        private float _lastCourseValue = -1f;

        private void OnEnable() {
            scoreStore.Changed += Refresh;
            GameEvents.Toast += ShowToast;
            Combo.Changed += OnCombo;
            Refresh();
            if (combo != null) combo.alpha = 0f;
            if (toast != null) toast.alpha = 0f;
        }

        private void OnDisable() {
            scoreStore.Changed -= Refresh;
            GameEvents.Toast -= ShowToast;
            Combo.Changed -= OnCombo;
        }

        private void Start() {
            _powerUps = FindFirstObjectByType<Vehicle.PlayerPowerUps>();
        }

        private void OnCombo(int multiplier) {
            if (combo == null) return;
            combo.text = $"×{multiplier}";
        }

        private void Refresh() {
            if (hiScore != null) hiScore.SetText("HI {0:0000000}", Mathf.Max(GameProgress.HighScore, scoreStore.PlayerOverallScore));
            for (var i = 0; i < lives.Count; i++)
                if (lives[i] != null) lives[i].color = UiColors.Regolith.WithAlpha(i < scoreStore.Lives ? 1f : 0.25f);
        }

        private void Update() {
            UpdatePowerUp();
            if (combo != null) combo.alpha = Mathf.MoveTowards(combo.alpha, Combo.Multiplier > 1 ? 1f : 0f, Time.deltaTime * 5f);
            if (courseBar == null || Mathf.Approximately(courseBar.value, _lastCourseValue)) return;
            _lastCourseValue = courseBar.value;
            foreach (var letter in courseLetters) {
                if (letter == null || !int.TryParse(letter.name, out var index)) continue;
                letter.color = index <= _lastCourseValue + 0.001f ? UiColors.Ink : UiColors.Dim;
            }
        }

        private void UpdatePowerUp() {
            if (powerUpLabel == null) return;
            var active = _powerUps != null ? _powerUps.Active : null;
            var shields = _powerUps != null ? _powerUps.ShieldCharges : 0;
            var name = active != null ? Vehicle.PlayerPowerUps.Name(active.Value) : shields > 0 ? $"SHIELD ×{shields}" : "NONE";
            powerUpLabel.text = $"POWER-UP · <color=#f0a13a><b>{name}</b></color>";
            var lit = active != null ? Mathf.CeilToInt(_powerUps.Remaining01 * powerUpPips.Count) : Mathf.Min(shields, powerUpPips.Count);
            for (var i = 0; i < powerUpPips.Count; i++) powerUpPips[i].color = i < lit ? UiColors.Ufo : UiColors.Hex(0x3a3228);
        }

        public void ShowToast(string title, string subtitle, float seconds) {
            if (toast == null) return;
            toastTitle.text = title;
            toastSubtitle.text = subtitle;
            if (_toastRoutine != null) StopCoroutine(_toastRoutine);
            _toastRoutine = StartCoroutine(ToastRoutine(seconds));
        }

        private IEnumerator ToastRoutine(float seconds) {
            yield return Fade(1f);
            yield return new WaitForSeconds(seconds);
            yield return Fade(0f);
        }

        private IEnumerator Fade(float target) {
            var start = toast.alpha;
            var rect = (RectTransform)toast.transform;
            for (var t = 0f; t < 0.25f; t += Time.unscaledDeltaTime) {
                var k = t / 0.25f;
                toast.alpha = Mathf.Lerp(start, target, k);
                rect.localScale = Vector3.one * Mathf.Lerp(0.9f, 1f, toast.alpha);
                yield return null;
            }

            toast.alpha = target;
            rect.localScale = Vector3.one * Mathf.Lerp(0.9f, 1f, target);
        }
    }
}
