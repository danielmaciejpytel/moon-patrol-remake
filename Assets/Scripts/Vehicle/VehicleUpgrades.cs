using ScriptableObjects.Upgrades;
using Settings;

namespace Vehicle {
    /// <summary>Efekty ulepszeń z garażu (poziomy z GameProgress).</summary>
    public static class VehicleUpgrades {
        public static int Level(UpgradeStat stat) {
            var levels = GameProgress.Data.upgradeLevels;
            var i = (int)stat;
            return i < levels.Count ? levels[i] : 0;
        }

        public static void SetLevel(UpgradeStat stat, int level) {
            var levels = GameProgress.Data.upgradeLevels;
            while (levels.Count <= (int)stat) levels.Add(0);
            levels[(int)stat] = level;
            GameProgress.Save();
        }

        /// <summary>Turbo cell: +5% prędkości maksymalnej na poziom (3 poziomy = +15%).</summary>
        public static float MaxSpeedMultiplier => 1f + 0.05f * Level(UpgradeStat.TurboCell);

        /// <summary>Hover tires: +10% wysokości skoku na poziom.</summary>
        public static float JumpMultiplier => 1f + 0.1f * Level(UpgradeStat.HoverTires);

        /// <summary>Twin cannon: podwójny strzał poziomy od 1. poziomu, każdy kolejny skraca przerwę między strzałami.</summary>
        public static bool TwinShot => Level(UpgradeStat.TwinCannon) > 0;

        public static float FireRateMultiplier => 1f + 0.2f * Level(UpgradeStat.TwinCannon);

        /// <summary>Shield dome: liczba trafień pochłanianych na etap.</summary>
        public static int ShieldCharges => Level(UpgradeStat.ShieldDome);
    }
}
