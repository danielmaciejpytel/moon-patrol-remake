using System;
using UnityEngine;

namespace ScriptableObjects.Score {
    /// <summary>
    /// Magazyn wyniku współdzielony między scenami. Zmiany:
    /// - wartości runtime są [NonSerialized] (nie zapisują się do assetu w edytorze);
    /// - event Changed → HUD aktualizuje się tylko przy zmianie, zamiast ToString() w każdej klatce;
    /// - ResetForNewGame() wywoływane tylko przy starcie nowej gry (wcześniej KAŻDY ScoreManager,
    ///   także ten doklejony do każdego kamienia, zerował wynik w Start()).
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/ScoreStore")]
    public class ScoreStoreSO : ScriptableObject {
        [NonSerialized] private int _playerOverallScore;
        [NonSerialized] private int _playerStageScore;
        [NonSerialized] private int _stageTime;

        [SerializeField] private int topRecord;
        [SerializeField] private int avgTime;
        [SerializeField] private int goodBonus;

        public event Action Changed;

        public int PlayerOverallScore => _playerOverallScore;
        public int PlayerStageScore => _playerStageScore;
        public int StageTime => _stageTime;
        public int TopRecord => topRecord;
        public int AvgTime => avgTime;
        public int GoodBonus => goodBonus;

        public void ResetForNewGame() {
            _playerOverallScore = 0;
            ResetForNewStage();
        }

        public void ResetForNewStage() {
            _playerStageScore = 0;
            _stageTime = 0;
            Changed?.Invoke();
        }

        /// <summary>Po śmierci: cofnij punkty z nieukończonego etapu, żeby ponowna próba startowała z wynikiem sprzed etapu.</summary>
        public void RevertStage() {
            _playerOverallScore -= _playerStageScore;
            ResetForNewStage();
        }

        public void AddScore(int points) {
            _playerOverallScore += points;
            _playerStageScore += points;
            Changed?.Invoke();
        }

        public void SetStageTime(int seconds) {
            if (seconds == _stageTime) return;
            _stageTime = seconds;
            Changed?.Invoke();
        }
    }
}
