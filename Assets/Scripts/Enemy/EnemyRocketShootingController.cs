using System.Collections;
using ScriptableObjects.Enemies;
using UnityEngine;

namespace Enemy {
    /// <summary>
    /// Zmiany:
    /// - flagi bool rocket/bomb/color/yellow/blue zastąpione EnemyType/EnemyProjectileKind w EnemyParamsSO
    ///   (wcześniej gdy żadna flaga nie była zaznaczona, _randomTime = 0 → strzał w pierwszej klatce);
    /// - czasy ataku i szansa z SO zamiast 8 pól readonly (w tym nieużywane _minTime/_maxTime);
    /// - korutyna zamiast licznika w FixedUpdate;
    /// - usunięte GetComponent&lt;ScoreManager&gt;() (wynik nieużywany) i no-op Mathf.Round(_randomTime) przed przypisaniem;
    /// - firePoint jako referencja w inspektorze zamiast transform.GetChild(0).
    /// </summary>
    public class EnemyRocketShootingController : MonoBehaviour {
        [SerializeField] private EnemyParamsSO enemyParamsSO;
        [SerializeField] private GameObject enemyProjectile;
        [Tooltip("Gdy puste – używany jest pierwszy child (zgodność z dotychczasowymi prefabami)")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private bool logDecisions;

        private IEnumerator Start() {
            if (firePoint == null && transform.childCount > 0) firePoint = transform.GetChild(0);

            var willShoot = Random.value < enemyParamsSO.AttackChance;
            var delay = Random.Range(enemyParamsSO.AttackDelayMin, enemyParamsSO.AttackDelayMax);

            if (logDecisions)
                Debug.Log($"{name} ({enemyParamsSO.Type}): {(willShoot ? $"{enemyParamsSO.ProjectileKind} in {delay:0.0}s" : "no attack")}", this);

            if (!willShoot || enemyProjectile == null) yield break;

            yield return new WaitForSeconds(delay);

            var origin = firePoint != null ? firePoint.position : transform.position;
            Instantiate(enemyProjectile, origin, Quaternion.identity);
        }
    }
}
