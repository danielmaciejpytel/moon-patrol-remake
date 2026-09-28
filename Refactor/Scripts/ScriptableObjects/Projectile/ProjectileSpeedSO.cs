using UnityEngine;

namespace ScriptableObjects.Projectile {
    [CreateAssetMenu(menuName = "ScriptableObjects/Projectile/ProjectileSpeed")]
    public class ProjectileSpeedSO : ScriptableObject {
        [SerializeField] private float projectileSpeed = 10f;

        public float ProjectileSpeed => projectileSpeed;
    }
}
