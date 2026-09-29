using UnityEngine;

namespace Camera {
    /// <summary>
    /// Parallax tła. Zmiany: referencja kamery z inspektora (fallback Camera.main) zamiast
    /// GameObject.Find("Main Camera"); LateUpdate (po ruchu kamery – brak drgań); czytelniejsze nazwy.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CameraParallaxEffect : MonoBehaviour {
        [Tooltip("0 = porusza się ze światem, 1 = przyklejone do kamery")]
        [SerializeField, Range(0f, 1f)] private float parallaxEffect;
        [SerializeField] private Transform cameraTransform;

        private float _length;
        private float _startX;

        private void Start() {
            if (cameraTransform == null) cameraTransform = UnityEngine.Camera.main.transform;
            _startX = transform.position.x;
            _length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void LateUpdate() {
            var camX = cameraTransform.position.x;
            var relativeToCamera = camX * (1f - parallaxEffect);
            var offset = camX * parallaxEffect;

            var position = transform.position;
            transform.position = new Vector3(_startX + offset, position.y, position.z);

            if (relativeToCamera > _startX + _length) _startX += _length;
            else if (relativeToCamera < _startX - _length) _startX -= _length;
        }
    }
}
