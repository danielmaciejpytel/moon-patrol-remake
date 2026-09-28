using Audio;

namespace Checkpoints {
    /// <summary>
    /// Zachowany tylko dla zgodności z obiektami w scenach (ten sam GUID skryptu).
    /// Docelowo: zamienić na CheckPointSound z sound = LastPoint i usunąć ten plik.
    /// </summary>
    public class FinalCheckpointSound : CheckPointSound {
        protected override SoundId Sound => SoundId.LastPoint;
    }
}
