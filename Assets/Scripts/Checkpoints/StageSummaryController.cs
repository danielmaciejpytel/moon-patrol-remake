using System.Collections.Generic;
using Scenes;
using ScriptableObjects.Course;
using ScriptableObjects.Scenes;
using ScriptableObjects.Score;
using Settings;
using TMPro;
using UI.Kit;
using UnityEngine;
using UnityEngine.UI;

namespace Checkpoints {
    /// <summary>
    /// Podsumowanie etapu jak w makiecie: czas kontra średnia i rekord, UFO, celność, bonus, ocena,
    /// mapa trasy A–Z i odliczanie do następnego etapu.
    /// Nazwa etapu z CourseSO zamiast Split(' ')[1] na nazwie sceny.
    /// </summary>
    public class StageSummaryController : MonoBehaviour {
        [SerializeField] private ScoreStoreSO scoreStore;
        [SerializeField] private ScenesSO scenesSO;
        [SerializeField] private CourseSO course;
        [SerializeField] private LevelController levelController;

        [Header("Texts")]
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text stageTime;
        [SerializeField] private TMP_Text averageTime;
        [SerializeField] private TMP_Text topRecord;
        [SerializeField] private TMP_Text ufos;
        [SerializeField] private TMP_Text accuracy;
        [SerializeField] private TMP_Text bonus;
        [SerializeField] private TMP_Text total;
        [SerializeField] private TMP_Text medal;
        [SerializeField] private TMP_Text medalCaption;
        [SerializeField] private TMP_Text nextStage;
        [SerializeField] private TMP_Text countdown;

        [Header("Course map (26 cells A–Z)")]
        [SerializeField] private List<Image> mapCells = new List<Image>();

        private float _leaveTime;
        private int _shown = -1;

        private void Start() {
            var index = course.IndexOfSummary(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            if (index < 0) index = Mathf.Max(0, scenesSO.CurrentStageIndex);
            var stage = course.Get(index);
            var next = course.Get(index + 1);

            var time = scoreStore.StageTime;
            var best = GameProgress.BestStageTime(index);
            var newRecord = GameProgress.RecordStageTime(index, time);
            GameProgress.ReachLetter(stage.toLetter);
            // Kredyty do garażu: punkty zdobyte w etapie (z bonusem) / 5.
            GameProgress.Data.credits += (scoreStore.PlayerStageScore + scoreStore.Bonus) / 5;
            GameProgress.Save();

            title.text = $"STAGE {stage.Label} CLEAR";
            stageTime.SetText("{0:000} s", time);
            averageTime.SetText("{0:000} s", stage.averageTime);
            topRecord.text = newRecord ? $"{time:000} s NEW" : $"{best:000} s";
            ufos.text = $"{scoreStore.UfosDestroyed} / {Mathf.Max(scoreStore.UfosSpawned, scoreStore.UfosDestroyed)}";
            accuracy.SetText("{0} %", scoreStore.Accuracy);
            bonus.text = scoreStore.Bonus > 0 ? $"+{scoreStore.Bonus}" : "0";
            bonus.color = scoreStore.Bonus > 0 ? UiColors.Good : UiColors.Dim;
            total.SetText("{0:0000000}", scoreStore.PlayerOverallScore);

            var diff = stage.averageTime - time;
            medal.text = Rank(diff, scoreStore.Accuracy);
            medalCaption.text = diff >= 0 ? $"RANK · BEAT AVG BY {diff} S" : $"RANK · {-diff} S OVER AVG";

            for (var i = 0; i < mapCells.Count; i++) {
                var cell = mapCells[i];
                var label = cell.GetComponentInChildren<TMP_Text>();
                var done = i < stage.toLetter;
                var here = i == stage.toLetter;
                cell.color = here ? UiColors.Ufo : done ? UiColors.Teal : UiColors.Hex(0x20262b);
                if (label != null) label.color = here ? UiColors.Hex(0x1d1206) : done ? UiColors.Ink : UiColors.Dim;
                var isStageEnd = false;
                foreach (var s in course.Stages) isStageEnd |= s.toLetter == i;
                var marker = cell.transform.Find("stage");
                if (marker != null) marker.gameObject.SetActive(isStageEnd);
            }

            nextStage.text = next != null ? next.Label : "FINISH";
            var delay = levelController != null ? levelController.NextSceneDelay : 5f;
            _leaveTime = Time.time + delay;
        }

        private void Update() {
            var left = Mathf.Max(0, Mathf.CeilToInt(_leaveTime - Time.time));
            if (left == _shown) return;
            _shown = left;
            countdown.SetText("{0}", left);
        }

        private static string Rank(int secondsUnderAverage, int accuracy) {
            if (secondsUnderAverage >= 0 && accuracy >= 60) return "A";
            if (secondsUnderAverage >= 0 || accuracy >= 60) return "B";
            return "C";
        }
    }
}
