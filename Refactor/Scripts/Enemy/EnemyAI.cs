using Core;
using ScriptableObjects.Enemies;
using UnityEngine;

namespace Enemy {
    /// <summary>
    /// Ruch UFO. Zmiany: parametry z EnemyParamsSO (wcześniej TODO + stałe w kodzie),
    /// CompareTag zamiast porównania nazwy "AiWalls", czytelny licznik zmiany kierunku.
    /// BŁĄD naprawiony: EnemyFlee() zmieniał _movementDirection, ale nie przeliczał _movementPerSecond,
    /// więc "uciekające" UFO leciało dalej starym torem.
    /// Ruch pozostaje przez transform w Update celowo: UFO są dziećmi AiWalls (poruszających się z kamerą),
    /// a ich Rigidbody2D jest Kinematic – MovePosition w przestrzeni świata walczyłoby z ruchem rodzica.
    /// Wymaga: obiekty ścian AI otagowane "AiWalls".
    /// </summary>
    [RequireComponent(typeof(PolygonCollider2D))]
    [DisallowMultipleComponent]
    public class EnemyAI : MonoBehaviour {
        [SerializeField] private EnemyParamsSO enemyParams;
        [Tooltip("Po ucieczce UFO jest niszczone po tym czasie od kontaktu ze ścianą")]
        [SerializeField, Min(0f)] private float destroyAfterFleeDelay = 2f;

        private Vector2 _velocity;
        private float _nextDirectionChangeTime;
        private bool _firstMove = true;
        private bool _fleeing;
        private bool _scheduledForDestroy;

        private float Speed => enemyParams != null ? enemyParams.MoveSpeed : 5f;
        private float VerticalSpeed => enemyParams != null ? enemyParams.VerticalSpeed : 1f;
        private float DirectionChangeTime => enemyParams != null ? enemyParams.DirectionChangeTime : 2f;

        public void EnemyFlee() {
            _fleeing = true;
            var fleeSpeed = enemyParams != null ? enemyParams.FleeSpeed : 15f;
            _velocity = new Vector2(Random.Range(-1f, 1f), 1f) * fleeSpeed;
        }

        private void Start() => PickNewDirection();

        private void Update() {
            transform.position += (Vector3)(_velocity * Time.deltaTime);

            if (!_fleeing && Time.time >= _nextDirectionChangeTime) PickNewDirection();
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (!collision.gameObject.CompareTag(GameTags.AiWalls)) return;

            if (_fleeing) {
                if (_scheduledForDestroy) return;
                _scheduledForDestroy = true;
                Destroy(gameObject, destroyAfterFleeDelay);
                return;
            }

            // Odbicie od ściany; przez jeden okres kierunek nie jest losowany ponownie.
            var bounce = enemyParams != null ? enemyParams.WallBounceSpeed : 4f;
            _velocity = new Vector2(-_velocity.x / Speed * bounce, -_velocity.y);
            _nextDirectionChangeTime = Time.time + DirectionChangeTime;
        }

        private void PickNewDirection() {
            // Na początku UFO zawsze opada w stronę gracza.
            var y = _firstMove ? -1f : Random.Range(-1f, 1f);
            _firstMove = false;
            _velocity = new Vector2(Random.Range(-1f, 1f) * Speed, y * VerticalSpeed);
            _nextDirectionChangeTime = Time.time + DirectionChangeTime;
        }
    }
}
