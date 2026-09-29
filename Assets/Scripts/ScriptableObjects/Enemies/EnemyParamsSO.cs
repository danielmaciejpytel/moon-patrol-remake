using UnityEngine;

namespace ScriptableObjects.Enemies {
    /// <summary>
    /// Wszystkie parametry UFO w jednym miejscu (wcześniej część była na sztywno w EnemyAI
    /// i EnemyRocketShootingController jako readonly float).
    /// Nazwy starych pól bez zmian – istniejące assety Blue_ufo / Color_Ufo / Yellow_Ufo zachowają dane.
    /// Nowe pola trzeba uzupełnić: Yellow 2–4 s, Color 2–5 s, Blue 6–10 s (wartości z dotychczasowego kodu).
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Enemies/Params")]
    public class EnemyParamsSO : ScriptableObject {
        [Header("Identity")]
        [SerializeField] private EnemyType type;
        [SerializeField] private Sprite sprite;

        [Header("Stats")]
        [SerializeField] private int HP = 1;
        [SerializeField] private int score = 100;
        [SerializeField] private float timeToFlee = 15f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [Tooltip("Prędkość pionowa (w oryginale nieskalowana: 1 j/s)")]
        [SerializeField] private float verticalSpeed = 1f;
        [SerializeField] private float fleeSpeed = 15f;
        [SerializeField] private float directionChangeTime = 2f;
        [Tooltip("Mnożnik prędkości poziomej po odbiciu od AiWalls")]
        [SerializeField] private float wallBounceSpeed = 4f;

        [Header("Attack")]
        [SerializeField] private EnemyProjectileKind projectileKind = EnemyProjectileKind.Rocket;
        [SerializeField, Range(0f, 1f)] private float attackChance = 0.7f;
        [SerializeField, Min(0f)] private float attackDelayMin = 2f;
        [SerializeField, Min(0f)] private float attackDelayMax = 4f;

        [Header("Unused / planned (dotychczasowe pola)")]
        [SerializeField] private float weaponRange;
        [SerializeField] private float visionRange;
        [SerializeField] private GameObject bulletPrefab1, bulletPrefab2, deathEffect;

        public EnemyType Type => type;
        public Sprite Sprite => sprite;
        public int Hp => HP;
        public int Score => score;
        public float TimeToFlee => timeToFlee;
        public float MoveSpeed => moveSpeed;
        public float VerticalSpeed => verticalSpeed;
        public float FleeSpeed => fleeSpeed;
        public float DirectionChangeTime => directionChangeTime;
        public float WallBounceSpeed => wallBounceSpeed;
        public EnemyProjectileKind ProjectileKind => projectileKind;
        public float AttackChance => attackChance;
        public float AttackDelayMin => attackDelayMin;
        public float AttackDelayMax => attackDelayMax;
        public GameObject DeathEffect => deathEffect;

        private void OnValidate() => attackDelayMax = Mathf.Max(attackDelayMax, attackDelayMin);
    }
}
