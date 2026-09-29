using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Kit {
    /// <summary>Pokazuje obrys/podświetlenie, gdy element (suwak, wiersz, zakładka) jest zaznaczony klawiaturą lub padem.</summary>
    public class UiFocus : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler {
        [SerializeField] private Graphic focusGraphic;
        [SerializeField] private bool selectOnHover = true;

        private void OnEnable() {
            if (focusGraphic != null) focusGraphic.enabled = EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject;
        }

        public void OnSelect(BaseEventData eventData) {
            if (focusGraphic != null) focusGraphic.enabled = true;
        }

        public void OnDeselect(BaseEventData eventData) {
            if (focusGraphic != null) focusGraphic.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (selectOnHover && EventSystem.current != null && TryGetComponent<Selectable>(out var s) && s.IsInteractable())
                EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
