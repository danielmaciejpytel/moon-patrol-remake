using UnityEngine;

namespace ScriptableObjects.Player {
    /// <summary>
    /// Nazwy pól serializowanych bez zmian → istniejący asset zachowuje wartości.
    /// Publiczne pola zamienione na prywatne [SerializeField] + właściwości tylko do odczytu
    /// (runtime nie może przypadkiem nadpisać danych assetu).
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Player/parameters")]
    public class PlayerParamsSO : ScriptableObject {
        [Tooltip("Prędkość bazowa, do której pojazd wraca po puszczeniu gazu/hamulca")]
        [SerializeField] private float playerSpeed = 4.5f;
        [Tooltip("Czas fazy wznoszenia skoku (s). Kolejny skok możliwy po 2× tym czasie.")]
        [SerializeField] private float jumpTimeInAir = 0.3f;
        [SerializeField] private float jumpHeightAccelerate = 6f;
        [SerializeField] private float jumpHorizontalSpeed = 4.5f;
        [SerializeField] private float minimalSpeed = 3f;
        [SerializeField] private float maxSpeed = 6f;
        [Tooltip("Zmiana prędkości na sekundę przy gazie/hamulcu i przy powrocie do bazowej")]
        [SerializeField] private float speedStep = 2f;
        [Tooltip("Stała prędkość docisku do podłoża (ujemna Y), gdy pojazd nie skacze")]
        [SerializeField] private float groundStickSpeed = 4f;

        public float PlayerSpeed => playerSpeed;
        public float JumpTimeInAir => jumpTimeInAir;
        public float JumpHeightAccelerate => jumpHeightAccelerate;
        public float JumpHorizontalSpeed => jumpHorizontalSpeed;
        public float MinimalSpeed => minimalSpeed;
        public float MaxSpeed => maxSpeed;
        public float SpeedStep => speedStep;
        public float GroundStickSpeed => groundStickSpeed;

        private void OnValidate() => maxSpeed = Mathf.Max(maxSpeed, minimalSpeed);
    }
}
