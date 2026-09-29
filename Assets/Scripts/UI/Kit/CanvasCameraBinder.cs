using UnityEngine;

namespace UI.Kit {
    /// <summary>
    /// Canvas w trybie Screen Space - Camera podpina kamerę główną sceny, dzięki czemu interfejs
    /// mieści się w obszarze 4:3 wyznaczonym przez CameraAspectRatio (letterbox), a nie na całym oknie.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CanvasCameraBinder : MonoBehaviour {
        [SerializeField] private float planeDistance = 1f;

        private void Awake() {
            var canvas = GetComponent<Canvas>();
            var cam = UnityEngine.Camera.main;
            if (cam == null) return;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = planeDistance;
        }
    }
}
