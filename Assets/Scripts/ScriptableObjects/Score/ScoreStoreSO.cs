using System;
using UnityEngine;

namespace ScriptableObjects.Score {
    /// <summary>
    /// Magazyn wyniku współdzielony między scenami. Zmiany:
    /// - wartości runtime są [NonSerialized] (nie zapisują się do assetu w edytorze);
    /// - event Changed → HUD aktualizuje się tylko przy zmianie, zamiast ToString() w każdej klatce;
    /// - ResetForNewGame() wywoływane tylko przy starcie nowej gry (wcześniej KAŻDY ScoreManager,
    ///   także ten doklejony do każdego kamienia, zerował wynik w Start());
    /// - statystyki etapu (UFO, celność) i życia do HUD i podsumowania etapu.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/ScoreStore")]
    public class ScoreStoreSO : ScriptableObject {
        public const int MaxLives = 3;

        [NonSerialized] private int _playerOverallScore;
        [NonSerialized] private int _playerStageScore;
        [NonSerialized] private int _stageStartScore;
        [NonSerialized] private int _stageTime;
        [NonSerialized] private int _lives = MaxLives;
        [NonSerialized] private int _ufosSpawned, _ufosDestroyed, _shotsFired, _shotsHit;
        [NonSerialized] private int _bonus;

        [SerializeField] private int topRecord;
        [SerializeField] private int avgTime;
        [SerializeField] private int goodBonus;

        public event Action Changed;

        public int PlayerOverallScore => _playerOverallScore;
        public int PlayerStageScore => _playerStageScore;
        public int StageStartScore => _stageStartScore;
        public int StageTime => _stageTime;
        public int Lives => _lives;
        public int UfosSpawned => _ufosSpawned;
        public int UfosDestroyed => _ufosDestroyed;
        public int ShotsFired => _shotsFired;
        public int ShotsHit => _shotsHit;
        public int Bonus => _bonus;
        public int Accuracy => _shotsFired > 0 ? Mathf.RoundToInt(100f * _shotsHit / _shotsFired) : 0;
        public int TopRecord => topRecord;
        public int AvgTime => avgTime;
        public int GoodBonus => goodBonus;

        /// <summary>Najlepszy wynik (z zapisu postępu) – do HUD i menu.</summary>
        [NonSerialized] public int HiScore;

        public void ResetForNewGame() {
            _playerOverallScore = 0;
            _lives = MaxLives;
            BeginStage();
        }

        /// <summary>Wznowienie gry z zapisu (Continue w menu).</summary>
        public void ResumeGame(int score) {
            _playerOverallScore = score;
            _lives = MaxLives;
            BeginStage();
        }

        /// <summary>Start etapu (punkt kontrolny): wynik z tej chwili jest punktem powrotu po śmierci, życia odnowione.</summary>
        public void BeginStage() {
            _stageStartScore = _playerOverallScore;
            _lives = MaxLives;
            ResetForNewStage();
        }

        /// <summary>Wejście w scenę etapu (także po śmierci) – zerowanie licznika czasu i statystyk etapu.</summary>
        public void ResetForNewStage() {
            _playerStageScore = 0;
            _stageTime = 0;
            _ufosSpawned = _ufosDestroyed = _shotsFired = _shotsHit = 0;
            _bonus = 0;
            Changed?.Invoke();
        }

        /// <summary>Po śmierci: wynik wraca do stanu z punktu kontrolnego, ubywa życie. Zwraca true, gdy życia się skończyły.</summary>
        public bool LoseLife() {
            _lives = Mathf.Max(0, _lives - 1);
            _playerOverallScore = _stageStartScore;
            var gameOver = _lives == 0;
            if (gameOver) {
                // Continue: etap od początku, wynik od zera, pełne życia.
                _playerOverallScore = 0;
                _stageStartScore = 0;
                _lives = MaxLives;
            }

            ResetForNewStage();
            return gameOver;
        }

        /// <summary>Zachowane dla zgodności: cofnięcie punktów z nieukończonego etapu.</summary>
        public void RevertStage() {
            _playerOverallScore = _stageStartScore;
            ResetForNewStage();
        }

        public void AddScore(int points) {
            _playerOverallScore += points;
            _playerStageScore += points;
            Changed?.Invoke();
        }

        public void AddBonus(int points) {
            _bonus = points;
            _playerOverallScore += points;
            Changed?.Invoke();
        }

        public void SetStageTime(int seconds) {
            if (seconds == _stageTime) return;
            _stageTime = seconds;
            Changed?.Invoke();
        }

        public void CountUfoSpawned() => _ufosSpawned++;
        public void CountUfoDestroyed() => _ufosDestroyed++;
        public void CountShotFired() => _shotsFired++;
        public void CountShotHit() => _shotsHit++;
    }
}
