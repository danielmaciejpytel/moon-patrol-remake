using Settings;
using TMPro;
using UnityEngine;

namespace UI.Kit {
    /// <summary>
    /// Przesunięcie kolorów (czerwona i turkusowa kopia tekstu pod spodem), jak klasa "ca" w makiecie.
    /// Wyłączane razem z efektem "Color bleeding" w ustawieniach.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class ChromaticText : MonoBehaviour {
        [SerializeField] private float offset = 1f;
        [SerializeField] private Color left = new Color(0.91f, 0.15f, 0.37f, 0.7f);
        [SerializeField] private Color right = new Color(0.26f, 0.82f, 0.75f, 0.6f);

        private TMP_Text _source;
        private TMP_Text _left, _right;
        private string _lastText;
        private float _lastSize;

        private void Awake() {
            _source = GetComponent<TMP_Text>();
            _left = CreateCopy("ca-left", left, -offset);
            _right = CreateCopy("ca-right", right, offset);
        }

        private void OnEnable() {
            GameSettings.Changed += ApplySetting;
            ApplySetting();
        }

        private void OnDisable() {
            GameSettings.Changed -= ApplySetting;
            SetCopiesActive(false);
        }

        private void OnDestroy() {
            if (_left != null) Destroy(_left.gameObject);
            if (_right != null) Destroy(_right.gameObject);
        }

        private void ApplySetting() => SetCopiesActive(GameSettings.Data.colorBleeding && isActiveAndEnabled);

        private void SetCopiesActive(bool value) {
            if (_left != null) _left.gameObject.SetActive(value);
            if (_right != null) _right.gameObject.SetActive(value);
        }

        private void LateUpdate() {
            if (_left == null || !_left.gameObject.activeSelf) return;
            if (_source.text == _lastText && Mathf.Approximately(_source.fontSize, _lastSize)) return;
            _lastText = _source.text;
            _lastSize = _source.fontSize;
            Sync(_left);
            Sync(_right);
        }

        private TMP_Text CreateCopy(string copyName, Color color, float dx) {
            var go = new GameObject(copyName, typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            var src = (RectTransform)transform;
            rt.SetParent(src.parent, false);
            rt.SetSiblingIndex(src.GetSiblingIndex());
            rt.anchorMin = src.anchorMin;
            rt.anchorMax = src.anchorMax;
            rt.pivot = src.pivot;
            rt.sizeDelta = src.sizeDelta;
            rt.anchoredPosition = src.anchoredPosition + new Vector2(dx, 0f);
            rt.localRotation = src.localRotation;
            rt.localScale = src.localScale;
            var t = go.AddComponent<TextMeshProUGUI>();
            t.raycastTarget = false;
            t.color = color;
            Sync(t);
            return t;
        }

        private void Sync(TMP_Text t) {
            t.font = _source.font;
            t.fontSize = _source.fontSize;
            t.fontStyle = _source.fontStyle;
            t.alignment = _source.alignment;
            t.characterSpacing = _source.characterSpacing;
            t.textWrappingMode = _source.textWrappingMode;
            t.overflowMode = _source.overflowMode;
            t.text = _source.text;
        }
    }
}
