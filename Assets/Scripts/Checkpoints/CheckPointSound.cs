using Audio;
using Obstacles;
using UnityEngine;

namespace Checkpoints {
    /// <summary>
    /// Dźwięk punktu kontrolnego. Zastępuje dwie identyczne klasy CheckPointSound i FinalCheckpointSound
    /// (różniły się tylko nazwą dźwięku) – dla ostatniego punktu ustaw sound = LastPoint.
    /// </summary>
    public class CheckPointSound : MonoBehaviour {
        [SerializeField] private SoundId sound = SoundId.Checkpoint;

        private bool _played;

        protected virtual SoundId Sound => sound;

        private void OnTriggerEnter2D(Collider2D other) {
            if (_played || !VehicleHitDetection.TryGetVehicle(other, out _)) return;
            _played = true;
            AudioManager.Play(Sound);
        }
    }
}
