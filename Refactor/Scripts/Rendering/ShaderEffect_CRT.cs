using UnityEngine;

/// <summary>
/// Efekt scanline CRT (Built-in Render Pipeline – OnRenderImage nie działa w URP; przy migracji na URP
/// trzeba przepisać na Renderer Feature / Full Screen Pass).
/// Zmiany: przeniesione z Assets/Resources/Scripts (skrypty w Resources niczego nie dają); shader jako
/// referencja (Shader.Find wymaga "Always Included" w buildzie); materiał niszczony w OnDisable
/// (z [ExecuteInEditMode] każdy reload tworzył nowy wyciekający materiał); ID właściwości cache'owane;
/// usunięte nieużywane pole material_Displacement. Klasa bez namespace – zgodnie z oryginałem (GUID/sceny).
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(UnityEngine.Camera))]
public class ShaderEffect_CRT : MonoBehaviour {
    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    private static readonly int ValueXId = Shader.PropertyToID("_ValueX");

    [SerializeField] private Shader scanlinesShader;
    [SerializeField] private float scanlineIntensity = 100;
    [SerializeField] private int scanlineWidth = 1;

    private Material _material;

    private void OnEnable() {
        if (scanlinesShader == null) scanlinesShader = Shader.Find("Hidden/Scanlines");
        if (scanlinesShader != null && _material == null)
            _material = new Material(scanlinesShader) { hideFlags = HideFlags.HideAndDontSave };
    }

    private void OnDisable() {
        if (_material == null) return;
        if (Application.isPlaying) Destroy(_material);
        else DestroyImmediate(_material);
        _material = null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_material == null) {
            Graphics.Blit(source, destination);
            return;
        }

        _material.SetFloat(IntensityId, scanlineIntensity * 0.01f);
        _material.SetFloat(ValueXId, scanlineWidth);
        Graphics.Blit(source, destination, _material);
    }
}
