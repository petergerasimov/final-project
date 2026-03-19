using UnityEngine;

public class FreezingToggle : MonoBehaviour
{
    private FreezingEffect m_freezingEffect;

    private void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (!player.activeSelf) continue;
            m_freezingEffect = player.GetComponent<FreezingEffect>();
            if (m_freezingEffect != null) break;
        }
    }

    private void OnEnable()
    {
        if (m_freezingEffect != null) m_freezingEffect.enabled = true;
    }

    private void OnDisable()
    {
        if (m_freezingEffect != null) m_freezingEffect.enabled = false;
    }
}
