using UnityEngine;

/// <summary>
/// Efekt rozmycia kolorów (Built-in RP). Te same poprawki co ShaderEffect_CRT: poza Resources,
/// shader jako referencja, niszczenie materiału, cache ID właściwości.
/// </summary>
[RequireComponent(typeof(UnityEngine.Camera))]
public class ShaderEffectBleedingColors : MonoBehaviour {
    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    private static readonly int ValueXId = Shader.PropertyToID("_ValueX");

    [SerializeField] private Shader bleedingShader;
    [SerializeField] private float intensity = 3;
    [SerializeField] private float shift = 0.5f;

    private Material _material;

    private void OnEnable() {
        if (bleedingShader == null) bleedingShader = Shader.Find("Hidden/BleedingColors");
        if (bleedingShader != null && _material == null)
            _material = new Material(bleedingShader) { hideFlags = HideFlags.HideAndDontSave };
    }

    private void OnDisable() {
        if (_material == null) return;
        Destroy(_material);
        _material = null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_material == null) {
            Graphics.Blit(source, destination);
            return;
        }

        _material.SetFloat(IntensityId, intensity);
        _material.SetFloat(ValueXId, shift);
        Graphics.Blit(source, destination, _material);
    }
}
