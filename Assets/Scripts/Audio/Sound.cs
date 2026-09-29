using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio {
    [Serializable]
    public class Sound {
        [Tooltip("Zostawione dla zgodności z danymi w AudioManager.prefab – mapowane na SoundId po nazwie.")]
        public string name;
        public SoundId id;
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool loop;

        // Domyślne wartości: wcześniej nowy wpis miał volume = 0 i pitch = 0 (dźwięk niesłyszalny).
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;

        [NonSerialized] public AudioSource Source;
    }
}
