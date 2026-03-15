using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhysX5ForUnity.PhysxTriangleMeshClothActor))]
[RequireComponent(typeof(MeshCollider))]
public class ClothCollisionHandler : MonoBehaviour
{
    public string playerTag = "Player";
    public float fadeDuration = 5f;

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
        
        StartCoroutine(FadeOut());
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

    private IEnumerator FadeOut()
    {
        Renderer clothRenderer = GetComponent<Renderer>();
        if (clothRenderer == null || clothRenderer.material == null) yield break;

        Material mat = clothRenderer.material;
        string colorProp = mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
        
        if (!mat.HasProperty(colorProp)) yield break;

        Color startColor = mat.GetColor(colorProp);
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, timeElapsed / fadeDuration);
            mat.SetColor(colorProp, new Color(startColor.r, startColor.g, startColor.b, alpha));
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
