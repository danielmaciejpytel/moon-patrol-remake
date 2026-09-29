using ScriptableObjects.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    /// <summary>
    /// Nazwa etapu w HUD. TextMeshPro; nazwa sceny brana bezpośrednio z aktywnej sceny, bo wcześniej
    /// wynik zależał od kolejności Start() względem LevelController (przy starcie poziomu wprost z edytora
    /// pokazywał się poprzedni etap).
    /// </summary>
    public class StageControllerUI : MonoBehaviour {
        [SerializeField] private ScenesSO scenesSO;
        [SerializeField] private TMP_Text stageHudText;

        private void Start() {
            // HUD jest też w menu (buildIndex 0) – tam pokazujemy ostatni etap z ScenesSO (lub nic).
            var scene = SceneManager.GetActiveScene();
            stageHudText.text = scene.buildIndex > 0 ? scene.name : scenesSO.CurrentStageLabel ?? string.Empty;
        }
    }
}
