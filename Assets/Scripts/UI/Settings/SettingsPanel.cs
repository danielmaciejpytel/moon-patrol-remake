using System.Collections.Generic;
using Settings;
using TMPro;
using UI.Kit;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings {
    /// <summary>
    /// Ustawienia z makiety: głośność muzyki i efektów, linie CRT, przesunięcie kolorów, wstrząs ekranu
    /// oraz zmiana klawiszy. Zmiany działają od razu i są zapisywane w settings.json.
    /// </summary>
    public class SettingsPanel : UiScreen {
        [Header("Tabs")]
        [SerializeField] private Button audioVideoTab;
        [SerializeField] private Button controlsTab;
        [SerializeField] private TMP_Text audioVideoLabel;
        [SerializeField] private TMP_Text controlsLabel;
        [SerializeField] private Graphic audioVideoUnderline;
        [SerializeField] private Graphic controlsUnderline;

        [Header("Audio")]
        [SerializeField] private Slider music;
        [SerializeField] private TMP_Text musicValue;
        [SerializeField] private Slider effects;
        [SerializeField] private TMP_Text effectsValue;

        [Header("Video")]
        [SerializeField] private Toggle crt;
        [SerializeField] private Toggle colorBleeding;
        [SerializeField] private Toggle screenShake;

        [Header("Controls")]
        [SerializeField] private List<RebindRow> rebindRows = new List<RebindRow>();

        private void Awake() {
            music.onValueChanged.AddListener(v => SetVolume(v, true));
            effects.onValueChanged.AddListener(v => SetVolume(v, false));
            crt.onValueChanged.AddListener(v => SetFlag(ref GameSettings.Data.crtScanlines, v));
            colorBleeding.onValueChanged.AddListener(v => SetFlag(ref GameSettings.Data.colorBleeding, v));
            screenShake.onValueChanged.AddListener(v => SetFlag(ref GameSettings.Data.screenShake, v));
            audioVideoTab.onClick.AddListener(() => ShowTab(false));
            controlsTab.onClick.AddListener(() => ShowTab(true));
        }

        protected override void OnOpened() {
            var s = GameSettings.Data;
            music.SetValueWithoutNotify(s.music * 100f);
            effects.SetValueWithoutNotify(s.effects * 100f);
            musicValue.SetText("{0}", Mathf.RoundToInt(s.music * 100f));
            effectsValue.SetText("{0}", Mathf.RoundToInt(s.effects * 100f));
            crt.SetIsOnWithoutNotify(s.crtScanlines);
            colorBleeding.SetIsOnWithoutNotify(s.colorBleeding);
            screenShake.SetIsOnWithoutNotify(s.screenShake);
            foreach (var t in new[] { crt, colorBleeding, screenShake })
                if (t.TryGetComponent<UiSwitch>(out var sw)) sw.Refresh();
            foreach (var row in rebindRows) row.Refresh();
            HighlightTab(false);
        }

        protected override void OnCancel() {
            if (RebindRow.IsListening) return;
            base.OnCancel();
        }

        private void ShowTab(bool controls) {
            HighlightTab(controls);
            Select(controls && rebindRows.Count > 0 ? rebindRows[0].gameObject : music.gameObject);
        }

        private void HighlightTab(bool controls) {
            audioVideoLabel.color = controls ? UiColors.Dim : UiColors.TealHi;
            controlsLabel.color = controls ? UiColors.TealHi : UiColors.Dim;
            audioVideoUnderline.enabled = !controls;
            controlsUnderline.enabled = controls;
        }

        private void SetVolume(float value, bool isMusic) {
            var v = Mathf.RoundToInt(value);
            if (isMusic) {
                GameSettings.Data.music = v / 100f;
                musicValue.SetText("{0}", v);
            }
            else {
                GameSettings.Data.effects = v / 100f;
                effectsValue.SetText("{0}", v);
            }

            GameSettings.Apply();
        }

        private static void SetFlag(ref bool field, bool value) {
            field = value;
            GameSettings.Apply();
        }
    }
}
