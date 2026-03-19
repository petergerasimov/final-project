using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light m_lightSource;
    [SerializeField] private Renderer m_emissiveRenderer;
    [SerializeField] private int m_materialIndex;

    [Header("Intensity")]
    [SerializeField] private float m_baseIntensity = 1f;
    [SerializeField] private float m_minIntensity = 0.2f;
    [SerializeField] private float m_maxIntensity = 1.5f;

    [Header("Flicker Behavior")]
    [SerializeField] private float m_primaryFrequency = 3f;
    [SerializeField] private float m_secondaryFrequency = 7f;
    [SerializeField] private float m_tertiaryFrequency = 13f;
    [SerializeField] private float m_noiseSpeed = 5f;
    [SerializeField] private float m_noiseStrength = 0.3f;
    [SerializeField] private float m_sineStrength = 0.4f;

    [Header("Random Dips")]
    [SerializeField][Range(0f, 1f)] private float m_dipChance = 0.03f;
    [SerializeField] private float m_dipStrength = 0.6f;
    [SerializeField] private float m_dipRecoverySpeed = 8f;

    [Header("Emission")]
    [SerializeField] private Color m_emissionColor = Color.yellow;

    private float m_noiseOffset;
    private float m_currentDip;
    private MaterialPropertyBlock m_propertyBlock;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private void Start()
    {
        m_noiseOffset = Random.Range(0f, 1000f);
        m_propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (m_lightSource == null || m_emissiveRenderer == null) return;

        float t = Time.time;

        float sine = Mathf.Sin(t * m_primaryFrequency) * 0.5f
                   + Mathf.Sin(t * m_secondaryFrequency) * 0.3f
                   + Mathf.Sin(t * m_tertiaryFrequency) * 0.2f;

        float noise = (Mathf.PerlinNoise(t * m_noiseSpeed + m_noiseOffset, m_noiseOffset) - 0.5f) * 2f;

        float flicker = m_baseIntensity + sine * m_sineStrength + noise * m_noiseStrength;

        if (Random.value < m_dipChance) m_currentDip = m_dipStrength;

        m_currentDip = Mathf.Lerp(m_currentDip, 0f, Time.deltaTime * m_dipRecoverySpeed);
        flicker -= m_currentDip;
        flicker = Mathf.Clamp(flicker, m_minIntensity, m_maxIntensity);

        m_lightSource.intensity = flicker;
        m_emissiveRenderer.GetPropertyBlock(m_propertyBlock, m_materialIndex);
        m_propertyBlock.SetColor(EmissionColorID, m_emissionColor * flicker);
        m_emissiveRenderer.SetPropertyBlock(m_propertyBlock, m_materialIndex);
    }
}
