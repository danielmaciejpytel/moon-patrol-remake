using System.Collections.Generic;
using GameInput;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Settings {
    /// <summary>
    /// Wiersz zmiany klawisza (Input System rebinding): Enter/A na wierszu → "PRESS A KEY…" → nowy klawisz
    /// zastępuje pierwsze przypisanie klawiatury. Pozostałe przypisania (drugi klawisz, pad) pokazane obok.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class RebindRow : MonoBehaviour {
        public enum Target {
            Accelerate,
            SlowDown,
            Jump,
            Fire,
            Pause
        }

        [SerializeField] private InputReaderSO input;
        [SerializeField] private Target target;
        [SerializeField] private List<TMP_Text> keyChips = new List<TMP_Text>();
        [SerializeField] private TMP_Text listening;

        private InputActionRebindingExtensions.RebindingOperation _operation;

        public static bool IsListening { get; private set; }

        private void Awake() => GetComponent<Button>().onClick.AddListener(StartRebind);

        private void OnEnable() => Refresh();

        private void OnDisable() => Cancel();

        private InputAction Action => target switch {
            Target.Accelerate or Target.SlowDown => input.ThrottleAction,
            Target.Jump => input.JumpAction,
            Target.Fire => input.ShootAction,
            _ => input.PauseAction
        };

        private bool Matches(InputBinding b) {
            if (b.isComposite) return false;
            if (target == Target.Accelerate) return !b.isPartOfComposite ? b.path.Contains("Gamepad") : b.name == "positive";
            if (target == Target.SlowDown) return !b.isPartOfComposite ? b.path.Contains("Gamepad") : b.name == "negative";
            return true;
        }

        public void Refresh() {
            var action = Action;
            if (listening != null) listening.gameObject.SetActive(false);
            var chip = 0;
            if (action != null)
                for (var i = 0; i < action.bindings.Count && chip < keyChips.Count; i++) {
                    var b = action.bindings[i];
                    if (!Matches(b)) continue;
                    var text = b.path.Contains("leftStick") ? "LS" : action.GetBindingDisplayString(i, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                    SetChip(chip++, Pretty(text));
                }

            for (; chip < keyChips.Count; chip++) SetChip(chip, null);
        }

        private void SetChip(int index, string text) {
            var chip = keyChips[index];
            var holder = chip.transform.parent.gameObject;
            holder.SetActive(!string.IsNullOrEmpty(text));
            chip.text = text;
        }

        private static string Pretty(string s) => s switch {
            "Right Arrow" or "Right" => "→",
            "Left Arrow" or "Left" => "←",
            "Up Arrow" or "Up" => "↑",
            "Down Arrow" or "Down" => "↓",
            "Space" => "SPACE",
            "Esc" or "Escape" => "ESC",
            _ => s.ToUpperInvariant()
        };

        private int FirstKeyboardBinding(InputAction action) {
            for (var i = 0; i < action.bindings.Count; i++) {
                var b = action.bindings[i];
                if (Matches(b) && b.effectivePath.StartsWith("<Keyboard>")) return i;
            }

            return -1;
        }

        private void StartRebind() {
            if (IsListening) return;
            var action = Action;
            var index = action != null ? FirstKeyboardBinding(action) : -1;
            if (index < 0) return;

            IsListening = true;
            foreach (var c in keyChips) c.transform.parent.gameObject.SetActive(false);
            if (listening != null) listening.gameObject.SetActive(true);

            action.Disable();
            _operation = action.PerformInteractiveRebinding(index)
                .WithControlsHavingToMatchPath("<Keyboard>")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(_ => Finish(true))
                .OnCancel(_ => Finish(false))
                .Start();
        }

        private void Finish(bool changed) {
            var action = Action;
            _operation?.Dispose();
            _operation = null;
            action?.Enable();
            if (changed) input.SaveBindingOverrides();
            Refresh();
            // Klawisz, który zakończył nasłuch, nie może od razu zamknąć panelu (Esc).
            StartCoroutine(ReleaseListening());
        }

        private System.Collections.IEnumerator ReleaseListening() {
            yield return null;
            yield return null;
            IsListening = false;
        }

        private void Cancel() {
            if (_operation == null) return;
            _operation.Cancel();
            _operation?.Dispose();
            _operation = null;
            IsListening = false;
        }
    }
}
