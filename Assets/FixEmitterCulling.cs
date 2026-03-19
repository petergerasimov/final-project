/* fixes physx particle emitter culling with per particle shader */

using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FixEmitterCulling : MonoBehaviour
{
    private Renderer m_renderer;
    private Bounds m_massiveBounds;

    void Start()
    {
        m_renderer = GetComponent<Renderer>();
        m_massiveBounds = new Bounds(Vector3.zero, new Vector3(10000000.0f, 10000000.0f, 10000000.0f));
    }

    void Update()
    {
        if (m_renderer == null) return;
        
        m_renderer.bounds = m_massiveBounds;
    }
}
