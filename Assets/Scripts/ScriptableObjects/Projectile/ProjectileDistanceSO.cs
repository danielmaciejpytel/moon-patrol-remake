using UnityEngine;

namespace ScriptableObjects.Projectile {
    /// <summary>
    /// Uwaga: wartość jest używana jako CZAS ŻYCIA pocisku w sekundach (Destroy(obj, t)),
    /// a nie dystans – nazwa zostaje, żeby nie gubić danych w assetach.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Projectile/ProjectileDistance")]
    public class ProjectileDistanceSO : ScriptableObject {
        [Tooltip("Czas życia pocisku (s)")]
        [SerializeField, Min(0.01f)] private float projectileDistance = 0.5f;

        public float Lifetime => projectileDistance;
    }
}
