using System;
using UnityEngine;

namespace ScriptableObjects.Scenes {
    /// <summary>
    /// Stan przejść między scenami. Usunięto pola typu UnityEngine.SceneManagement.Scene
    /// (struktura nie jest serializowalna – w inspektorze była zawsze pusta i nieużywana).
    /// Stan runtime jest [NonSerialized], więc nie "brudzi" assetu w edytorze między sesjami Play Mode.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenes")]
    public class ScenesSO : ScriptableObject {
        [Header("Scene names (Build Settings)")]
        [SerializeField] private string menuScene = "StartMenuv2.0";
        [SerializeField] private string gameOverScene = "DeadPlayerMenu";
        [SerializeField] private string gameFinishedScene = "GameFinished";

        [Tooltip("Etykieta etapu wyświetlana w HUD i podsumowaniu, np. A-E")]
        [NonSerialized] public string CurrentStageLabel;
        [Tooltip("Ostatni poziom rozgrywki – do niego wraca ekran Game Over")]
        [NonSerialized] public string LastPlayedLevel;
        [NonSerialized] public string CurrentScene;
        [NonSerialized] public int CurrentSceneIndex;
        [Tooltip("Indeks etapu w CourseSO (-1 poza etapem)")]
        [NonSerialized] public int CurrentStageIndex = -1;
        [Tooltip("Ponowne wejście w etap po utracie życia (bez nowego punktu kontrolnego)")]
        [NonSerialized] public bool Retrying;
        [NonSerialized] public bool ContinuedAfterGameOver;
        [Tooltip("Łączny czas przejazdu (s) – do rankingu")]
        [NonSerialized] public int RunTime;

        public string MenuScene => menuScene;
        public string GameOverScene => gameOverScene;
        public string GameFinishedScene => gameFinishedScene;
    }
}
