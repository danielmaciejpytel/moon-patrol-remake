namespace Audio {
    /// <summary>
    /// Identyfikatory dźwięków – zamiast literałów Play("Jump") rozsianych po 10 plikach
    /// (literówka = cichy błąd w runtime). Nazwy odpowiadają obecnym wpisom w AudioManager.prefab.
    /// </summary>
    public enum SoundId {
        Theme,
        Jump,
        Blaster,
        Hit,
        Crash,
        Enemy,
        Checkpoint,
        LastPoint,
        GameOver,
        Intro,
        Start
    }
}
