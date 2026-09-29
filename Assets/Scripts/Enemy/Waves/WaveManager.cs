namespace Enemy.Waves {
    /// <summary>
    /// Zachowany dla zgodności z obiektami w scenach (ten sam GUID skryptu).
    /// Duplikował WaveStarter – różnił się tylko spawnem w pozycji wyzwalacza.
    /// Docelowo: zamienić komponenty na WaveStarter ze SpawnMode.AtTriggerPosition i usunąć ten plik.
    /// </summary>
    public class WaveManager : WaveStarter {
        protected override SpawnMode Mode => SpawnMode.AtTriggerPosition;
    }
}
