using UnityEngine;
using UnityEngine.Pool;

namespace Projectiles {
    /// <summary>
    /// Baza pocisku obsługiwanego przez UnityEngine.Pool.ObjectPool.
    /// Zastępuje Instantiate(Resources.Load(...)) + Destroy(obj, t) przy każdym strzale.
    /// Despawn() jest idempotentne (trafienie i koniec czasu życia w tej samej klatce nie zwolnią obiektu 2×).
    /// </summary>
    public abstract class PooledProjectile : MonoBehaviour {
        private IObjectPool<PooledProjectile> _pool;
        private float _despawnTime;
        private bool _active;

        public void SetPool(IObjectPool<PooledProjectile> pool) => _pool = pool;

        public virtual void Launch(Vector3 position, float lifetime) {
            transform.SetPositionAndRotation(position, Quaternion.identity);
            _despawnTime = Time.time + lifetime;
            _active = true;
        }

        protected virtual void Update() {
            if (_active && Time.time >= _despawnTime) Despawn();
        }

        public void Despawn() {
            if (!_active) return;
            _active = false;

            if (_pool != null) _pool.Release(this);
            else Destroy(gameObject);
        }
    }
}
