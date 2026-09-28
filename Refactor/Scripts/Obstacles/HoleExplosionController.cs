using UnityEngine;

namespace Obstacles {
    /// <summary>
    /// Dziura po bombie wroga. Oryginał w Start niszczył EdgeCollider2D, dodawał BoxCollider2D przez AddComponent,
    /// po czym robił GetComponent, i modyfikował lokalną kopię position, która nigdy nie była przypisana (martwy kod).
    /// Docelowo najlepiej ustawić BoxCollider2D (trigger) wprost w prefabie i usunąć ten skrypt.
    /// </summary>
    public class HoleExplosionController : MonoBehaviour {
        [SerializeField] private Vector2 triggerSize = new Vector2(4f, 2f);

        private void Awake() {
            if (TryGetComponent(out EdgeCollider2D edge)) Destroy(edge);
            if (!TryGetComponent(out BoxCollider2D box)) box = gameObject.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = triggerSize;
        }
    }
}
