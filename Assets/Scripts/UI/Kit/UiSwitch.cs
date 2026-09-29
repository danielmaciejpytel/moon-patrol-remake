using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Kit {
    /// <summary>Wygląd przełącznika z makiety dla Toggle: tor zmienia kolor, gałka przesuwa się w prawo.</summary>
    [RequireComponent(typeof(Toggle))]
    public class UiSwitch : MonoBehaviour, ISelectHandler, IDeselectHandler {
        [SerializeField] private Graphic track;
        [SerializeField] private RectTransform knob;
        [SerializeField] private Graphic knobGraphic;
        [SerializeField] private Graphic focusOutline;
        [SerializeField] private float travel = 22f;

        private Toggle _toggle;

        private void Awake() {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(_ => Refresh());
        }

        private void OnEnable() {
            if (focusOutline != null) focusOutline.enabled = false;
            Refresh();
        }

        public void Refresh() {
            if (_toggle == null) _toggle = GetComponent<Toggle>();
            var on = _toggle.isOn;
            if (track != null) track.color = on ? UiColors.Teal : UiColors.Hex(0x2f363c);
            if (knob != null) knob.anchoredPosition = new Vector2(on ? travel : 0f, knob.anchoredPosition.y);
            if (knobGraphic != null) knobGraphic.color = on ? UiColors.Ink : UiColors.Regolith;
        }

        public void OnSelect(BaseEventData eventData) {
            if (focusOutline != null) focusOutline.enabled = true;
        }

        public void OnDeselect(BaseEventData eventData) {
            if (focusOutline != null) focusOutline.enabled = false;
        }
    }
}
