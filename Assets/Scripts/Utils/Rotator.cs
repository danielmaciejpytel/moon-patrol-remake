using UnityEngine;

namespace Utils {
    /// <summary>
    /// Jeden komponent obrotu zamiast trzech kopii tego samego kodu:
    /// Enemy/BlueRolling.cs, Utils/Ufo_03_rotate.cs (klasa "rotation" – nazwa niezgodna z plikiem, więc Unity
    /// nie pozwala jej podpiąć) i SpritesFinal/enemies/ufo_03/ufo_03_spinning.cs (skrypt w folderze grafik,
    /// nieużywany). Komentarz "50 degrees per second" nie zgadzał się z wartością 180.
    /// </summary>
    public class Rotator : MonoBehaviour {
        [Tooltip("Stopnie na sekundę wokół osi Z")]
        [SerializeField] private float degreesPerSecond = 180f;

        private void Update() => transform.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime);
    }
}
