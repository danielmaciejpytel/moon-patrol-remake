using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Audio {
    /// <summary>
    /// Trwały menedżer audio. Zmiany względem wersji "Brackeys":
    /// - jedna instancja przez całą grę (DontDestroyOnLoad) – muzyka nie restartuje się przy każdej scenie,
    ///   duplikaty z kolejnych scen są usuwane;
    /// - słownik SoundId → Sound zamiast Array.Find po stringu przy każdym strzale;
    /// - statyczny dostęp AudioManager.Play(...) zamiast FindObjectOfType w 10 miejscach;
    /// - czyszczenie statycznej instancji (działa z wyłączonym Domain Reload).
    /// </summary>
    [DisallowMultipleComponent]
    public class AudioManager : MonoBehaviour {
        [SerializeField] private Sound[] sounds = Array.Empty<Sound>();
        [SerializeField] private SoundId music = SoundId.Theme;
        [SerializeField] private bool playMusicOnStart = true;

        private static AudioManager _instance;
        private readonly Dictionary<SoundId, Sound> _lookup = new Dictionary<SoundId, Sound>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => _instance = null;

        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var sound in sounds) {
                if (sound.clip == null) continue;
                // Zgodność ze starymi danymi: jeśli wpis ma nazwę pasującą do enuma, użyj jej.
                if (!string.IsNullOrEmpty(sound.name) && Enum.TryParse(sound.name, out SoundId parsed)) sound.id = parsed;

                sound.Source = gameObject.AddComponent<AudioSource>();
                sound.Source.clip = sound.clip;
                sound.Source.volume = sound.volume;
                sound.Source.pitch = sound.pitch;
                sound.Source.loop = sound.loop;
                sound.Source.outputAudioMixerGroup = sound.mixerGroup;
                sound.Source.playOnAwake = false;

                if (!_lookup.TryAdd(sound.id, sound))
                    Debug.LogWarning($"{nameof(AudioManager)}: duplicate sound id {sound.id}", this);
            }
        }

        private void Start() {
            if (_instance != this) return;
            ApplyVolumes();
            GameSettings.Changed += ApplyVolumes;
            if (playMusicOnStart) PlayInternal(music);
        }

        private void OnDestroy() {
            if (_instance != this) return;
            GameSettings.Changed -= ApplyVolumes;
            _instance = null;
        }

        /// <summary>Głośność z ustawień gracza: dźwięki zapętlone to muzyka, pozostałe to efekty.</summary>
        private void ApplyVolumes() {
            var settings = GameSettings.Data;
            foreach (var sound in _lookup.Values)
                sound.Source.volume = sound.volume * (sound.loop ? settings.music : settings.effects);
        }

        public static void Play(SoundId id) {
            if (_instance == null) {
                Debug.LogWarning($"{nameof(AudioManager)} missing in scene, cannot play {id}");
                return;
            }

            _instance.PlayInternal(id);
        }

        private void PlayInternal(SoundId id) {
            if (!_lookup.TryGetValue(id, out var sound)) {
                Debug.LogWarning($"{nameof(AudioManager)}: sound {id} not found", this);
                return;
            }

            if (sound.loop) {
                if (!sound.Source.isPlaying) sound.Source.Play();
            }
            else {
                // PlayOneShot pozwala nakładać się krótkim efektom (seria strzałów, kilka trafień naraz).
                sound.Source.PlayOneShot(sound.clip);
            }
        }
    }
}
