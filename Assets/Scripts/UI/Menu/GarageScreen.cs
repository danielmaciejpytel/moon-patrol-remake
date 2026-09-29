using System;
using System.Collections.Generic;
using ScriptableObjects.Course;
using ScriptableObjects.Upgrades;
using Settings;
using TMPro;
using UI.Kit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vehicle;

namespace UI.Menu {
    /// <summary>
    /// Garaż: ulepszenia pojazdu kupowane za kredyty (punkty z etapów). Wybrane ulepszenie pokazuje na paskach
    /// statystyk, co zmieni (turkusowy odcinek). Niektóre ulepszenia odblokowuje dopiero punkt kontrolny.
    /// </summary>
    public class GarageScreen : UiScreen {
        [Serializable]
        public class UpgradeRow {
            public UpgradeSO upgrade;
            public Button button;
            public Graphic frame;
            public Graphic background;
            public TMP_Text cost;
            public List<Graphic> levels = new List<Graphic>();
        }

        [Serializable]
        public class StatBar {
            public RectTransform fill;
            public RectTransform preview;
            public TMP_Text value;
        }

        [SerializeField] private TMP_Text credits;
        [SerializeField] private List<UpgradeRow> upgrades = new List<UpgradeRow>();
        [SerializeField] private StatBar speed, jump, fireRate, armor;

        private UpgradeRow _selected;

        private void Awake() {
            foreach (var row in upgrades) {
                var r = row;
                r.button.onClick.AddListener(() => Buy(r));
                var trigger = r.button.gameObject.AddComponent<EventTrigger>();
                var select = new EventTrigger.Entry { eventID = EventTriggerType.Select };
                select.callback.AddListener(_ => {
                    _selected = r;
                    Refresh();
                });
                trigger.triggers.Add(select);
                var hover = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                hover.callback.AddListener(_ => Select(r.button.gameObject));
                trigger.triggers.Add(hover);
            }
        }

        protected override void OnOpened() {
            _selected = upgrades.Count > 0 ? upgrades[0] : null;
            FirstSelected = _selected?.button.gameObject;
            Refresh();
        }

        private static bool Unlocked(UpgradeSO u) => GameProgress.Data.highestLetter >= u.UnlockLetter;

        private void Buy(UpgradeRow row) {
            var u = row.upgrade;
            var level = VehicleUpgrades.Level(u.Stat);
            if (!Unlocked(u) || level >= u.MaxLevel || GameProgress.Data.credits < u.CostPerLevel) return;
            GameProgress.Data.credits -= u.CostPerLevel;
            VehicleUpgrades.SetLevel(u.Stat, level + 1);
            Audio.AudioManager.Play(Audio.SoundId.Checkpoint);
            Refresh();
        }

        private void Refresh() {
            credits.text = $"{GameProgress.Data.credits:N0} CR".Replace(',', ' ');
            foreach (var row in upgrades) {
                var u = row.upgrade;
                var level = VehicleUpgrades.Level(u.Stat);
                var unlocked = Unlocked(u);
                var on = row == _selected;
                row.frame.color = on ? UiColors.TealHi : UiColors.Line;
                row.background.color = on ? UiColors.Hex(0x16292a) : UiColors.Hex(0x1a1f24);
                row.cost.text = !unlocked ? $"CHECKPOINT {CourseSO.Letter(u.UnlockLetter)}" : level >= u.MaxLevel ? "MAX" : $"{u.CostPerLevel} CR";
                var group = row.button.GetComponent<CanvasGroup>();
                if (group != null) group.alpha = unlocked ? 1f : 0.55f;
                for (var i = 0; i < row.levels.Count; i++) row.levels[i].color = i < level ? UiColors.TealHi : UiColors.Hex(0x2f363c);
            }

            var preview = _selected != null && Unlocked(_selected.upgrade) && VehicleUpgrades.Level(_selected.upgrade.Stat) < _selected.upgrade.MaxLevel
                ? _selected.upgrade.Stat
                : (UpgradeStat?)null;
            SetBar(speed, 55 + 10 * VehicleUpgrades.Level(UpgradeStat.TurboCell), preview == UpgradeStat.TurboCell ? 10 : 0);
            SetBar(jump, 40 + 15 * VehicleUpgrades.Level(UpgradeStat.HoverTires), preview == UpgradeStat.HoverTires ? 15 : 0);
            SetBar(fireRate, 50 + 20 * VehicleUpgrades.Level(UpgradeStat.TwinCannon), preview == UpgradeStat.TwinCannon ? 20 : 0);
            SetBar(armor, 20 + 25 * VehicleUpgrades.Level(UpgradeStat.ShieldDome), preview == UpgradeStat.ShieldDome ? 25 : 0);
        }

        private static void SetBar(StatBar bar, int value, int delta) {
            value = Mathf.Min(100, value);
            delta = Mathf.Min(100 - value, delta);
            bar.fill.anchorMax = new Vector2(value / 100f, 1f);
            bar.preview.anchorMin = new Vector2(value / 100f, 0f);
            bar.preview.anchorMax = new Vector2((value + delta) / 100f, 1f);
            bar.value.text = (value + delta).ToString();
            bar.value.color = delta > 0 ? UiColors.TealHi : UiColors.Ink;
        }
    }
}
