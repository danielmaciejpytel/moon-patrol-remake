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

        public string MenuScene => menuScene;
        public string GameOverScene => gameOverScene;
        public string GameFinishedScene => gameFinishedScene;
    }
}
