using UnityEngine;

namespace Camera {
    /// <summary>
    /// Letterbox / pillarbox do proporcji 4:3. Zmiany: proporcja w inspektorze, przeliczenie także po zmianie
    /// rozmiaru okna (wcześniej tylko w Start), bez duplikacji gałęzi.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraAspectRatio : MonoBehaviour {
        [SerializeField] private Vector2 targetAspect = new Vector2(4f, 3f);

        private UnityEngine.Camera _camera;
        private int _lastWidth, _lastHeight;

        private void Awake() => _camera = GetComponent<UnityEngine.Camera>();

        private void Update() {
            if (Screen.width == _lastWidth && Screen.height == _lastHeight) return;
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            Apply();
        }

        private void Apply() {
            var target = targetAspect.x / targetAspect.y;
            var window = Screen.width / (float)Screen.height;
            var scaleHeight = window / target;

            _camera.rect = scaleHeight < 1f
                ? new Rect(0f, (1f - scaleHeight) / 2f, 1f, scaleHeight)
                : new Rect((1f - 1f / scaleHeight) / 2f, 0f, 1f / scaleHeight, 1f);
        }
    }
}
