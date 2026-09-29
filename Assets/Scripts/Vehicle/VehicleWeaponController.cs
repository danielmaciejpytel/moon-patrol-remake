using Audio;
using GameInput;
using Projectiles;
using Projectiles.Player;
using ScriptableObjects.Projectile;
using UnityEngine;
using UnityEngine.Pool;

namespace Vehicle {
    /// <summary>
    /// Zmiany:
    /// - prefaby pocisków jako referencje w inspektorze zamiast Resources.Load przy każdym strzale;
    /// - ObjectPool zamiast Instantiate/Destroy;
    /// - wejście przez InputReaderSO (event), bez Input.GetKeyDown w Update;
    /// - wyjście z gry (Esc) przeniesione do LevelController/MenuStart – wcześniej było za "return"
    ///   blokady strzelania, więc w menu (napis "Press ESC to exit") Esc nie działał.
    /// </summary>
    public class VehicleWeaponController : MonoBehaviour {
        [SerializeField] private InputReaderSO input;

        [Header("Fire points")]
        [SerializeField] private Transform firePointHorizon;
        [SerializeField] private Transform firePointVertical;

        [Header("Projectiles")]
        [SerializeField] private HorizonBullet horizontalBulletPrefab;
        [SerializeField] private VerticalBullet verticalBulletPrefab;
        [SerializeField] private ProjectileDistanceSO horizonDistanceRange;
        [SerializeField] private ProjectileDistanceSO verticalDistanceRange;
        [SerializeField, Min(1)] private int poolMaxSize = 16;

        [SerializeField] private bool shootingEnabled = true;

        private ObjectPool<PooledProjectile> _horizontalPool;
        private ObjectPool<PooledProjectile> _verticalPool;

        public void SetShootingEnabled(bool value) => shootingEnabled = value;

        /// <summary>Zachowane dla zgodności z dotychczasowym wywołaniem z MenuStart.</summary>
        public void VehicleInMenu() => SetShootingEnabled(false);

        private void Awake() {
            _horizontalPool = CreatePool(horizontalBulletPrefab);
            _verticalPool = CreatePool(verticalBulletPrefab);
        }

        private void OnEnable() {
            if (input != null) input.ShootPressed += Shoot;
        }

        private void OnDisable() {
            if (input != null) input.ShootPressed -= Shoot;
        }

        private void OnDestroy() {
            _horizontalPool?.Dispose();
            _verticalPool?.Dispose();
        }

        /// <summary>Strzał zlecony z kodu (autopilot w menu/demo) – działa także przy wyłączonym strzelaniu gracza.</summary>
        public void FireFromCode() => Fire();

        private void Shoot() {
            if (!shootingEnabled) return;
            Fire();
        }

        private void Fire() {

            var horizontal = _horizontalPool.Get();
            horizontal.Launch(firePointHorizon.position, horizonDistanceRange.Lifetime);

            var vertical = (VerticalBullet)_verticalPool.Get();
            vertical.SetFirePoint(firePointVertical);
            vertical.Launch(firePointVertical.position, verticalDistanceRange.Lifetime);

            AudioManager.Play(SoundId.Blaster);
            Core.GameEvents.RaiseShotFired();
        }

        private ObjectPool<PooledProjectile> CreatePool(PooledProjectile prefab) {
            ObjectPool<PooledProjectile> pool = null;
            pool = new ObjectPool<PooledProjectile>(
                createFunc: () => {
                    var instance = Instantiate(prefab);
                    instance.SetPool(pool);
                    return instance;
                },
                actionOnGet: p => p.gameObject.SetActive(true),
                actionOnRelease: p => p.gameObject.SetActive(false),
                actionOnDestroy: p => {
                    if (p != null) Destroy(p.gameObject);
                },
                collectionCheck: false,
                defaultCapacity: 4,
                maxSize: poolMaxSize);
            return pool;
        }
    }
}
