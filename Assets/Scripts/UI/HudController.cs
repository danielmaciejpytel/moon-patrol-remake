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

        private Coroutine _toastRoutine;
        private float _lastCourseValue = -1f;

        private void OnEnable() {
            scoreStore.Changed += Refresh;
            GameEvents.Toast += ShowToast;
            Refresh();
            if (toast != null) toast.alpha = 0f;
        }

        private void OnDisable() {
            scoreStore.Changed -= Refresh;
            GameEvents.Toast -= ShowToast;
        }

        private void Refresh() {
            if (hiScore != null) hiScore.SetText("HI {0:0000000}", Mathf.Max(GameProgress.HighScore, scoreStore.PlayerOverallScore));
            for (var i = 0; i < lives.Count; i++)
                if (lives[i] != null) lives[i].color = UiColors.Regolith.WithAlpha(i < scoreStore.Lives ? 1f : 0.25f);
        }

        private void Update() {
            if (courseBar == null || Mathf.Approximately(courseBar.value, _lastCourseValue)) return;
            _lastCourseValue = courseBar.value;
            foreach (var letter in courseLetters) {
                if (letter == null || !int.TryParse(letter.name, out var index)) continue;
                letter.color = index <= _lastCourseValue + 0.001f ? UiColors.Ink : UiColors.Dim;
            }
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
