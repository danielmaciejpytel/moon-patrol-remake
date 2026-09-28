using System;
using UnityEngine;

namespace Core {
    /// <summary>
    /// Globalne zdarzenia rozgrywki. Zastępują GameObject.Find("ScoreManager") / FindObjectOfType
    /// wywoływane przez przeszkody, wrogów i pojazd.
    /// Statyczne eventy są czyszczone przy starcie Play Mode, więc działa to także
    /// z wyłączonym Domain Reload (Enter Play Mode Options).
    /// </summary>
    public static class GameEvents {
        /// <summary>Przyznanie punktów (wróg zestrzelony, kamień zniszczony, przeskok przeszkody).</summary>
        public static event Action<int> ScoreAwarded;

        /// <summary>Gracz zginął (wywoływane dokładnie raz na życie).</summary>
        public static event Action PlayerDied;

        public static void AwardScore(int points) {
            if (points != 0) ScoreAwarded?.Invoke(points);
        }

        public static void RaisePlayerDied() => PlayerDied?.Invoke();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() {
            ScoreAwarded = null;
            PlayerDied = null;
        }
    }
}
