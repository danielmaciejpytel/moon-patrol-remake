using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings {
    [Serializable]
    public class RankEntry {
        public string pilot = "AAA";
        public int score;
        [Tooltip("Indeks ostatniej osiągniętej litery (0 = A)")]
        public int reached;
        [Tooltip("Łączny czas przejazdu (s)")]
        public int time;
        public long timestamp;
        [Tooltip("Etap, na którym skończył się przejazd (np. E–J)")]
        public string stage = "";
    }

    [Serializable]
    public class ProgressData {
        [Header("Continue")]
        public bool hasRun;
        public int stageIndex;
        public int score;
        public int runTime;

        [Header("Records")]
        public List<int> bestStageTimes = new List<int>();
        public List<RankEntry> ranking = new List<RankEntry>();

        [Header("Garage")]
        public int credits;
        public List<int> upgradeLevels = new List<int>();
        public int highestLetter;
    }

    /// <summary>
    /// Zapis postępu (punkt kontrolny do Continue, rekordy etapów, ranking, garaż) jako JSON w persistentDataPath.
    /// </summary>
    public static class GameProgress {
        private const string FileName = "progress.json";
        public const int RankingSize = 7;

        private static ProgressData _data;

        public static ProgressData Data => _data ??= JsonStorage.Load<ProgressData>(FileName);

        public static RankEntry Best => Data.ranking.Count > 0 ? Data.ranking[0] : null;

        public static void Save() => JsonStorage.Save(FileName, Data);

        /// <summary>Checkpoint na literze głównej: od tego etapu i z tym wynikiem wznawia Continue.</summary>
        public static void SetCheckpoint(int stageIndex, int score, int runTime) {
            Data.hasRun = true;
            Data.stageIndex = stageIndex;
            Data.score = score;
            Data.runTime = runTime;
            Save();
        }

        public static void ClearRun() {
            Data.hasRun = false;
            Save();
        }

        public static void ReachLetter(int letter) {
            if (letter <= Data.highestLetter) return;
            Data.highestLetter = letter;
            Save();
        }

        /// <summary>Zwraca true, jeśli czas jest nowym rekordem etapu.</summary>
        public static bool RecordStageTime(int stageIndex, int seconds) {
            while (Data.bestStageTimes.Count <= stageIndex) Data.bestStageTimes.Add(0);
            var best = Data.bestStageTimes[stageIndex];
            if (best != 0 && best <= seconds) return false;
            Data.bestStageTimes[stageIndex] = seconds;
            Save();
            return true;
        }

        public static int BestStageTime(int stageIndex) =>
            stageIndex >= 0 && stageIndex < Data.bestStageTimes.Count ? Data.bestStageTimes[stageIndex] : 0;

        public static int HighScore => Best?.score ?? 0;

        /// <summary>Miejsce, które zająłby wynik w rankingu (0..RankingSize-1) albo -1.</summary>
        public static int RankFor(int score) {
            if (score <= 0) return -1;
            var i = 0;
            while (i < Data.ranking.Count && Data.ranking[i].score >= score) i++;
            return i < RankingSize ? i : -1;
        }

        public static void AddRank(RankEntry entry) {
            var i = RankFor(entry.score);
            if (i < 0) return;
            entry.timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Data.ranking.Insert(i, entry);
            if (Data.ranking.Count > RankingSize) Data.ranking.RemoveRange(RankingSize, Data.ranking.Count - RankingSize);
            Save();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => _data = null;
    }
}
