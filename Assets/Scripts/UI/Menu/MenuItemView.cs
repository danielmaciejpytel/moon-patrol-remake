using TMPro;
using UI.Kit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Menu {
    /// <summary>
    /// Pozycja listy menu jak w makiecie: zaznaczenie = turkusowy gradient, trójkątny znacznik i jaśniejszy tekst.
    /// Działa z klawiaturą, padem (nawigacja EventSystem) i myszą (najechanie zaznacza).
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class MenuItemView : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler {
        [SerializeField] private Graphic highlight;
        [SerializeField] private Graphic marker;
        [SerializeField] private TMP_Text label;
        [SerializeField] private TMP_Text detail;

        private Button _button;
        private bool _selected;

        public Button Button => _button != null ? _button : _button = GetComponent<Button>();
        public TMP_Text Label => label;

        public bool Interactable {
            get => Button.interactable;
            set {
                Button.interactable = value;
                Refresh();
            }
        }

        public void SetDetail(string text) {
            if (detail == null) return;
            detail.text = text;
            detail.gameObject.SetActive(!string.IsNullOrEmpty(text));
        }

        private void OnEnable() => Refresh();

        public void OnSelect(BaseEventData eventData) {
            _selected = true;
            Refresh();
        }

        public void OnDeselect(BaseEventData eventData) {
            _selected = false;
            Refresh();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (Button.IsInteractable() && EventSystem.current != null) EventSystem.current.SetSelectedGameObject(gameObject);
        }

        private void Refresh() {
            var interactable = Button.IsInteractable();
            var on = _selected && interactable;
            if (highlight != null) highlight.enabled = on;
            if (marker != null) marker.enabled = on;
            if (label != null) label.color = !interactable ? UiColors.Dim.WithAlpha(0.6f) : on ? UiColors.Ink : UiColors.Regolith;
        }
    }
}
