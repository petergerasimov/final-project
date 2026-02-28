/* fixes physx particle emitter culling with per particle shader */

using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FixEmitterCulling : MonoBehaviour
{
    private Renderer m_Renderer;
    private Bounds m_MassiveBounds;

    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        m_MassiveBounds = new Bounds(Vector3.zero, new Vector3(10000000.0f, 10000000.0f, 10000000.0f));
    }

    void Update()
    {
        if (m_Renderer != null)
        {
            m_Renderer.bounds = m_MassiveBounds;
        }
    }
}
