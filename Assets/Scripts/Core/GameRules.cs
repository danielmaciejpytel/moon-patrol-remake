namespace Core {
    /// <summary>Zasady bieżącego etapu ustawiane przez LevelController na podstawie CourseSO.</summary>
    public static class GameRules {
        /// <summary>Czy bomby zostawiają kratery (tylko ostatni etap, jak w oryginale).</summary>
        public static bool BombCraters { get; set; }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => BombCraters = false;
    }
}
