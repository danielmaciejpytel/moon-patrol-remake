using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace UI.Kit {
    /// <summary>
    /// Ekran/nakładka interfejsu (menu, ustawienia, pauza, garaż, ranking) na wspólnym stosie:
    /// tylko górny ekran reaguje na "wstecz" (Esc / B na padzie), a po zamknięciu wraca zaznaczenie poprzedniego.
    /// </summary>
    public class UiScreen : MonoBehaviour {
        [SerializeField] private GameObject firstSelected;
        [Tooltip("Czy Esc / B zamyka ten ekran")]
        [SerializeField] private bool closeOnCancel = true;

        private static readonly List<UiScreen> Stack = new List<UiScreen>();

        private GameObject _returnSelection;
        private Action _onClosed;
        private int _openedFrame = -1;

        public bool IsOpen => Stack.Contains(this);
        public static UiScreen Top => Stack.Count > 0 ? Stack[Stack.Count - 1] : null;

        public GameObject FirstSelected {
            get => firstSelected;
            set => firstSelected = value;
        }

        public void Open(Action onClosed = null) {
            _onClosed = onClosed;
            if (!IsOpen) {
                _returnSelection = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
                Stack.Add(this);
            }

            _openedFrame = Time.frameCount;
            gameObject.SetActive(true);
            OnOpened();
            Select(firstSelected);
        }

        public void Close() {
            if (!IsOpen) return;
            Stack.Remove(this);
            gameObject.SetActive(false);
            OnClosedInternal();
            Select(_returnSelection);
            var callback = _onClosed;
            _onClosed = null;
            callback?.Invoke();
        }

        protected virtual void OnOpened() { }
        protected virtual void OnClosedInternal() { }

        /// <summary>Wstecz na górnym ekranie. Domyślnie zamyka ekran.</summary>
        protected virtual void OnCancel() {
            if (closeOnCancel) Close();
        }

        protected virtual void Update() {
            if (Top == this && Time.frameCount > _openedFrame + 1 && CancelPressed()) OnCancel();
        }

        protected virtual void OnDestroy() => Stack.Remove(this);

        public static void Select(GameObject go) {
            if (EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(null);
            if (go != null && go.activeInHierarchy) EventSystem.current.SetSelectedGameObject(go);
        }

        public static bool CancelPressed() {
            var module = EventSystem.current != null ? EventSystem.current.currentInputModule as InputSystemUIInputModule : null;
            return module != null && module.cancel != null && module.cancel.action != null && module.cancel.action.WasPressedThisFrame();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => Stack.Clear();
    }
}
