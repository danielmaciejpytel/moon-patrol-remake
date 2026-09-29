using System;
using System.IO;
using UnityEngine;

namespace Settings {
    /// <summary>Zapis i odczyt danych jako JSON w Application.persistentDataPath.</summary>
    public static class JsonStorage {
        public static T Load<T>(string fileName) where T : new() {
            var path = PathOf(fileName);
            try {
                if (File.Exists(path)) return JsonUtility.FromJson<T>(File.ReadAllText(path)) ?? new T();
            }
            catch (Exception e) {
                Debug.LogWarning($"{nameof(JsonStorage)}: cannot read {path}: {e.Message}");
            }

            return new T();
        }

        public static void Save<T>(string fileName, T data) {
            var path = PathOf(fileName);
            try {
                File.WriteAllText(path, JsonUtility.ToJson(data, true));
            }
            catch (Exception e) {
                Debug.LogWarning($"{nameof(JsonStorage)}: cannot write {path}: {e.Message}");
            }
        }

        public static void Delete(string fileName) {
            var path = PathOf(fileName);
            if (File.Exists(path)) File.Delete(path);
        }

        private static string PathOf(string fileName) => Path.Combine(Application.persistentDataPath, fileName);
    }
}
