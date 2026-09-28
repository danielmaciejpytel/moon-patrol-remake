using System.Collections;
using TMPro;
using UnityEngine;

namespace Text {
    /// <summary>
    /// Efekt maszyny do pisania. Oryginał był martwy (ustawianie tekstu zakomentowane, "TODO REMOVE"),
    /// a komponent i tak wisiał w 4 scenach, alokując Substring co krok.
    /// Teraz: TMP maxVisibleCharacters – zero alokacji, działa z rich text.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class TypeWriterEffect : MonoBehaviour {
        [SerializeField, Min(0f)] private float delay = 0.1f;
        [Tooltip("Pusty = użyj tekstu ustawionego w komponencie TMP")]
        [SerializeField, TextArea] private string fullText;

        private TMP_Text _label;

        private void Awake() => _label = GetComponent<TMP_Text>();

        private void OnEnable() => StartCoroutine(ShowText());

        private IEnumerator ShowText() {
            if (!string.IsNullOrEmpty(fullText)) _label.text = fullText;
            _label.ForceMeshUpdate();

            var total = _label.textInfo.characterCount;
            var wait = new WaitForSeconds(delay);
            for (var i = 0; i <= total; i++) {
                _label.maxVisibleCharacters = i;
                yield return wait;
            }
        }
    }
}
