using UnityEngine;

namespace ScriptableObjects.Upgrades {
    public enum UpgradeStat {
        TwinCannon,
        HoverTires,
        TurboCell,
        ShieldDome
    }

    /// <summary>
    /// Ulepszenie pojazdu z garażu, kupowane za kredyty (punkty z etapów). Poziomy zapisywane w GameProgress,
    /// efekty liczy VehicleUpgrades – bez zmian w PlayerParamsSO i w logice ruchu.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Upgrade")]
    public class UpgradeSO : ScriptableObject {
        [SerializeField] private UpgradeStat stat;
        [SerializeField] private string displayName = "Twin cannon";
        [SerializeField, TextArea] private string description = "Two horizontal shots.";
        [SerializeField, Min(0)] private int costPerLevel = 900;
        [SerializeField, Range(1, 5)] private int maxLevel = 3;
        [Tooltip("Litera trasy, którą trzeba osiągnąć, żeby odblokować (0 = od razu, 14 = O)")]
        [SerializeField, Range(0, 25)] private int unlockLetter;

        public UpgradeStat Stat => stat;
        public string DisplayName => displayName;
        public string Description => description;
        public int CostPerLevel => costPerLevel;
        public int MaxLevel => maxLevel;
        public int UnlockLetter => unlockLetter;
    }
}
