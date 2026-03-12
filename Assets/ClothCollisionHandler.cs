using UnityEngine;

[RequireComponent(typeof(PhysX5ForUnity.PhysxTriangleMeshClothActor))]
[RequireComponent(typeof(MeshCollider))]
public class ClothCollisionHandler : MonoBehaviour
{
    public string playerTag = "Player";

    private PhysX5ForUnity.PhysxTriangleMeshClothActor m_clothActor;
    private MeshCollider m_collider;
    private bool m_upperHalfTriggered = false;


    private void Start()
    {
        m_clothActor = GetComponent<PhysX5ForUnity.PhysxTriangleMeshClothActor>();
        m_collider = GetComponent<MeshCollider>();
        m_clothActor.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_upperHalfTriggered) return;
        if (!collision.gameObject.CompareTag(playerTag)) return;
        if (!IsContactInUpperHalf(collision)) return;

        m_upperHalfTriggered = true;
        m_clothActor.enabled = true;
        m_collider.enabled = false;
    }

    private bool IsContactInUpperHalf(Collision collision)
    {
        Bounds worldBounds = m_collider.bounds;
        float worldCenterY = worldBounds.center.y;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.point.y >= worldCenterY) return true;
        }
        return false;
    }
}
