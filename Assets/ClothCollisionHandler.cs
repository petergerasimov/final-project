using UnityEngine;

[RequireComponent(typeof(PhysX5ForUnity.PlanePhysxClothActor))]
[RequireComponent(typeof(MeshCollider))]
public class ClothCollisionHandler : MonoBehaviour
{
    public string playerTag = "Player";

    private PhysX5ForUnity.PlanePhysxClothActor m_clothActor;
    private MeshCollider m_collider;
    private Mesh m_colliderMesh;
    private bool m_upperHalfTriggered = false;


    private void Start()
    {
        m_clothActor = GetComponent<PhysX5ForUnity.PlanePhysxClothActor>();
        m_collider = GetComponent<MeshCollider>();

        Mesh original = m_collider.sharedMesh;
        m_colliderMesh = new Mesh
        {
            vertices = original.vertices,
            triangles = original.triangles,
            normals = original.normals,
            uv = original.uv
        };
        m_colliderMesh.RecalculateBounds();
        m_collider.sharedMesh = m_colliderMesh;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_upperHalfTriggered) return;
        if (!collision.gameObject.CompareTag(playerTag)) return;
        if (!IsContactInUpperHalf(collision)) return;
        
        m_upperHalfTriggered = true;
        m_clothActor.ReleaseUpperHalfBoundary();
        ShrinkColliderToLowerHalf();
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

    private void ShrinkColliderToLowerHalf()
    {
        Vector3[] verts = m_colliderMesh.vertices;
        int[] tris = m_colliderMesh.triangles;

        float minY = float.MaxValue, maxY = float.MinValue;
        Vector3[] worldVerts = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            worldVerts[i] = transform.TransformPoint(verts[i]);
            if (worldVerts[i].y < minY) minY = worldVerts[i].y;
            if (worldVerts[i].y > maxY) maxY = worldVerts[i].y;
        }
        float centerY = (minY + maxY) * 0.5f;

        int[] newTris = new int[tris.Length];
        for (int i = 0; i < tris.Length; i += 3)
        {
            float centroidY = (worldVerts[tris[i]].y + worldVerts[tris[i + 1]].y + worldVerts[tris[i + 2]].y) / 3f;
            if (centroidY < centerY)
            {
                newTris[i] = tris[i];
                newTris[i + 1] = tris[i + 1];
                newTris[i + 2] = tris[i + 2];
            }
        }

        Mesh lowerMesh = new Mesh
        {
            vertices = verts,
            triangles = newTris,
            normals = m_colliderMesh.normals,
            uv = m_colliderMesh.uv
        };
        lowerMesh.RecalculateBounds();

        m_collider.sharedMesh = null;
        m_collider.sharedMesh = lowerMesh;
    }
}
