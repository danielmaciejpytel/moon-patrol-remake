using System;
using UnityEngine;

namespace Core {
    /// <summary>
    /// Mnożnik punktów za trafienia jedno po drugim (UFO, kamienie, zestrzelone pociski wroga):
    /// kolejne trafienie w ciągu 2,2 s zwiększa mnożnik, maksymalnie ×4. Śmierć zeruje combo.
    /// </summary>
    public static class Combo {
        public const float Window = 2.2f;
        public const int MaxMultiplier = 4;

        private static int _count;
        private static float _expires;

        /// <summary>Aktualny mnożnik (1 = brak combo) – HUD pokazuje go od ×2.</summary>
        public static event Action<int> Changed;

        public static int Multiplier => Time.time < _expires ? Mathf.Clamp(_count, 1, MaxMultiplier) : 1;

        public static float TimeLeft => Mathf.Max(0f, _expires - Time.time);

        /// <summary>Punkty za trafienie z mnożnikiem combo.</summary>
        public static void AwardHit(int points) {
            _count = Time.time < _expires ? _count + 1 : 1;
            _expires = Time.time + Window;
            GameEvents.AwardScore(points * Multiplier);
            Changed?.Invoke(Multiplier);
        }

        public static void Reset() {
            _count = 0;
            _expires = 0f;
            Changed?.Invoke(1);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() {
            _count = 0;
            _expires = 0f;
            Changed = null;
        }
    }
}
