using UnityEngine;

public class FreezingPostProcess : MonoBehaviour
{
    private Material m_material;

    public Material Material
    {
        get { return m_material; }
    }

    private void Start()
    {
        Shader shader = Shader.Find("Custom/FreezingVignette");
        if (shader != null && shader.isSupported)
        {
            m_material = new Material(shader);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_material != null)
        {
            Graphics.Blit(source, destination, m_material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void OnDestroy()
    {
        if (m_material != null) Destroy(m_material);
    }
}
