using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Course;
using Settings;
using TMPro;
using UI.Kit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Menu {
    /// <summary>
    /// Ranking z inicjałami jak w automacie: ↑↓ zmienia literę, Enter przechodzi do następnej.
    /// Kolumna "Reached" pokazuje ostatni punkt kontrolny. Filtry: cały czas / ten tydzień (←→).
    /// </summary>
    public class RankingScreen : UiScreen, IMoveHandler, ISubmitHandler {
        [Serializable]
        public class Row {
            public Graphic highlight;
            public TMP_Text position, pilot, score, reached, time;
        }

        [SerializeField] private List<Row> rows = new List<Row>();
        [SerializeField] private TMP_Text allTimeFilter;
        [SerializeField] private TMP_Text weekFilter;
        [SerializeField] private TMP_Text hints;
        [SerializeField] private Selectable inputTarget;

        private bool _weekOnly;
        private RankEntry _pending;
        private int _pendingIndex = -1;
        private readonly char[] _initials = { 'A', 'A', 'A' };
        private int _letter;

        /// <summary>Otwiera ranking z wpisywaniem inicjałów dla nowego wyniku.</summary>
        public void OpenWithEntry(RankEntry entry, Action onClosed = null) {
            _pending = entry;
            _letter = 0;
            for (var i = 0; i < 3; i++) _initials[i] = 'A';
            _weekOnly = false;
            Open(onClosed);
        }

        protected override void OnOpened() {
            if (_pending == null) _weekOnly = false;
            FirstSelected = inputTarget.gameObject;
            Refresh();
        }

        protected override void OnCancel() {
            if (_pending != null) Commit();
            base.OnCancel();
        }

        public void OnMove(AxisEventData eventData) {
            if (_pending != null) {
                if (eventData.moveDir == MoveDirection.Up) Shift(1);
                else if (eventData.moveDir == MoveDirection.Down) Shift(-1);
                else if (eventData.moveDir == MoveDirection.Right) NextLetter();
                else if (eventData.moveDir == MoveDirection.Left) _letter = Mathf.Max(0, _letter - 1);
            }
            else if (eventData.moveDir == MoveDirection.Left || eventData.moveDir == MoveDirection.Right) {
                _weekOnly = !_weekOnly;
            }

            Refresh();
        }

        public void OnSubmit(BaseEventData eventData) {
            if (_pending != null) NextLetter();
            else Close();
            Refresh();
        }

        private void Shift(int delta) {
            var c = _initials[_letter] - 'A';
            _initials[_letter] = (char)('A' + (c + delta + 26) % 26);
        }

        private void NextLetter() {
            if (_letter < 2) _letter++;
            else Commit();
        }

        private void Commit() {
            if (_pending == null) return;
            _pending.pilot = new string(_initials);
            GameProgress.AddRank(_pending);
            _pending = null;
        }

        private void Refresh() {
            allTimeFilter.color = _weekOnly ? UiColors.Dim : UiColors.TealHi;
            weekFilter.color = _weekOnly ? UiColors.TealHi : UiColors.Dim;
            hints.text = _pending != null ? "↑ ↓  Letter      ENTER  Next letter" : "← →  Filter      ESC  Back";

            var weekAgo = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeSeconds();
            var list = GameProgress.Data.ranking.Where(r => !_weekOnly || r.timestamp >= weekAgo).ToList();
            _pendingIndex = -1;
            if (_pending != null) {
                _pendingIndex = list.TakeWhile(r => r.score >= _pending.score).Count();
                list.Insert(_pendingIndex, _pending);
            }

            for (var i = 0; i < rows.Count; i++) {
                var row = rows[i];
                var entry = i < list.Count ? list[i] : null;
                var me = i == _pendingIndex;
                row.highlight.enabled = me;
                row.position.text = entry != null ? (i + 1).ToString() : "";
                row.position.color = i == 0 ? UiColors.Ufo : UiColors.Dim;
                row.pilot.text = entry == null ? "" : me ? PilotWithCursor() : entry.pilot;
                row.score.text = entry != null ? entry.score.ToString("0000000") : "";
                row.reached.text = entry != null ? CourseSO.Letter(entry.reached).ToString() : "";
                row.time.text = entry != null ? $"{entry.time / 60:00}:{entry.time % 60:00}" : "";
                var color = me ? UiColors.Ink : UiColors.Regolith;
                row.pilot.color = row.score.color = row.reached.color = row.time.color = color;
            }
        }

        private string PilotWithCursor() {
            var s = "";
            for (var i = 0; i < 3; i++)
                s += i == _letter ? $"<color=#43d1c0><u>{_initials[i]}</u></color>" : _initials[i].ToString();
            return s;
        }
    }
}
