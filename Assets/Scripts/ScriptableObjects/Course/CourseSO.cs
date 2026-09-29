using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Course {
    /// <summary>
    /// Trasa A–Z podzielona na etapy (A–E, E–J, J–O, O–T, T–Z) – dane dla HUD, podsumowania, menu i zapisu postępu.
    /// Punkty kontrolne (checkpointy) to litery główne na początku każdego etapu.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Course")]
    public class CourseSO : ScriptableObject {
        public const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const int LastLetter = 25;

        [Serializable]
        public class Stage {
            [Tooltip("Nazwa sceny poziomu w Build Settings")]
            public string scene = "A-E";
            [Tooltip("Scena podsumowania po etapie")]
            public string summaryScene = "StageSummary A-E";
            [Range(0, LastLetter)] public int fromLetter;
            [Range(0, LastLetter)] public int toLetter = 4;
            [Tooltip("Pozycja X pojazdu na starcie i na końcu poziomu (pasek postępu)")]
            public float startX = -24f;
            public float endX = 466f;
            [Tooltip("Średni czas przejazdu (s) – podstawa bonusu jak w oryginale")]
            public int averageTime = 120;
            public int goodBonus = 1000;
            [Tooltip("Tło etapu (opis w HUD po checkpoincie)")]
            public string info = "MOUNTAINS";
            [Tooltip("Bomby blue UFO zostawiają kratery (w oryginale dopiero w ostatnim etapie)")]
            public bool bombCraters;

            public string Label => $"{Letters[fromLetter]}–{Letters[toLetter]}";
        }

        [SerializeField] private List<Stage> stages = new List<Stage>();

        public IReadOnlyList<Stage> Stages => stages;

        public int IndexOfScene(string sceneName) => stages.FindIndex(s => s.scene == sceneName);

        public int IndexOfSummary(string sceneName) => stages.FindIndex(s => s.summaryScene == sceneName);

        public Stage Get(int index) => index >= 0 && index < stages.Count ? stages[index] : null;

        public static char Letter(int index) => Letters[Mathf.Clamp(index, 0, LastLetter)];
    }
}
