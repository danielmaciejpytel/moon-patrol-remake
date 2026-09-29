using System;
using UnityEngine;

namespace Settings {
    [Serializable]
    public class SettingsData {
        [Range(0f, 1f)] public float music = 0.7f;
        [Range(0f, 1f)] public float effects = 0.85f;
        public bool crtScanlines = true;
        public bool colorBleeding = true;
        public bool screenShake = true;
        [Tooltip("Nadpisania klawiszy z Input System (SaveBindingOverridesAsJson)")]
        public string bindingOverrides = "";
    }

    /// <summary>
    /// Ustawienia gracza (głośność, efekty ekranu, klawisze) zapisywane jako JSON w persistentDataPath.
    /// Komponenty reagują na event Changed (AudioManager, efekty kamery, tekst z przesunięciem kolorów).
    /// </summary>
    public static class GameSettings {
        private const string FileName = "settings.json";

        private static SettingsData _data;

        public static event Action Changed;

        public static SettingsData Data => _data ??= JsonStorage.Load<SettingsData>(FileName);

        /// <summary>Po zmianie wartości w Data: powiadom słuchaczy i zapisz.</summary>
        public static void Apply(bool save = true) {
            Changed?.Invoke();
            if (save) JsonStorage.Save(FileName, Data);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() {
            _data = null;
            Changed = null;
        }
    }
}
