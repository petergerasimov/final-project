using UnityEngine;

public class RotationTransfer : MonoBehaviour
{
    [SerializeField] private Transform m_source;
    [SerializeField] private Transform m_target;

    void Update()
    {
        if (m_source == null || m_target == null) return;
        m_target.rotation = m_source.rotation;
    }
}
