using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light lightSource;
    [SerializeField] private Renderer emissiveRenderer;
    [SerializeField] private int materialIndex = 0;

    [Header("Intensity")]
    [SerializeField] private float baseIntensity = 1f;
    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 1.5f;

    [Header("Flicker Behavior")]
    [SerializeField] private float primaryFrequency = 3f;
    [SerializeField] private float secondaryFrequency = 7.3f;
    [SerializeField] private float tertiaryFrequency = 13.1f;
    [SerializeField] private float noiseSpeed = 5f;
    [SerializeField] private float noiseStrength = 0.3f;
    [SerializeField] private float sineStrength = 0.4f;

    [Header("Random Dips")]
    [SerializeField][Range(0f, 1f)] private float dipChance = 0.03f;
    [SerializeField] private float dipStrength = 0.6f;
    [SerializeField] private float dipRecoverySpeed = 8f;

    [Header("Emission")]
    [SerializeField] private Color emissionColor = Color.yellow;

    private float noiseOffset;
    private float currentDip;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private void Start()
    {
        noiseOffset = Random.Range(0f, 1000f);
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (lightSource == null || emissiveRenderer == null) return;

        float t = Time.time;

        float sine = Mathf.Sin(t * primaryFrequency) * 0.5f
                   + Mathf.Sin(t * secondaryFrequency) * 0.3f
                   + Mathf.Sin(t * tertiaryFrequency) * 0.2f;

        float noise = (Mathf.PerlinNoise(t * noiseSpeed + noiseOffset, noiseOffset) - 0.5f) * 2f;

        float flicker = baseIntensity + sine * sineStrength + noise * noiseStrength;

        if (Random.value < dipChance) currentDip = dipStrength;

        currentDip = Mathf.Lerp(currentDip, 0f, Time.deltaTime * dipRecoverySpeed);
        flicker -= currentDip;
        flicker = Mathf.Clamp(flicker, minIntensity, maxIntensity);

        lightSource.intensity = flicker;
        emissiveRenderer.GetPropertyBlock(propertyBlock, materialIndex);
        propertyBlock.SetColor(EmissionColorID, emissionColor * flicker);
        emissiveRenderer.SetPropertyBlock(propertyBlock, materialIndex);
    }
}
