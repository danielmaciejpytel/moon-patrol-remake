using ScriptableObjects.Scenes;
using ScriptableObjects.Score;
using TMPro;
using UnityEngine;

namespace Checkpoints {
    /// <summary>
    /// Ekran podsumowania etapu. Zmiany: TextMeshPro zamiast legacy UI Text,
    /// etykieta etapu z ScenesSO.CurrentStageLabel zamiast Split(' ')[1] na nazwie sceny
    /// (IndexOutOfRange, gdy nazwa nie zawiera spacji; ukryta zależność od LevelController).
    /// </summary>
    public class StageSummaryController : MonoBehaviour {
        [SerializeField] private ScoreStoreSO scoreStore;
        [SerializeField] private ScenesSO scenesSO;
        [SerializeField] private TMP_Text stagePoint;
        [SerializeField] private TMP_Text timeTextField;
        [SerializeField] private TMP_Text totalScoreOverall;

        private void Start() {
            stagePoint.text = scenesSO.CurrentStageLabel;
            timeTextField.SetText("{0}", scoreStore.StageTime);
            totalScoreOverall.SetText("{0}", scoreStore.PlayerOverallScore);
        }
    }
}
