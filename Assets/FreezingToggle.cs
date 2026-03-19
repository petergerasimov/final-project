using UnityEngine;

public class FreezingToggle : MonoBehaviour
{
    private FreezingEffect m_freezingEffect;
    private FreezingPostProcess m_postProcess;

    private void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (!player.activeSelf) continue;
            m_freezingEffect = player.GetComponent<FreezingEffect>();
            m_postProcess = player.GetComponent<FreezingPostProcess>();
            break;
        }
    }

    private void OnEnable()
    {
        if (m_freezingEffect != null) m_freezingEffect.enabled = true;
        if (m_postProcess != null) m_postProcess.enabled = true;
    }

    private void OnDisable()
    {
        if (m_freezingEffect != null) m_freezingEffect.enabled = false;
        if (m_postProcess != null) m_postProcess.enabled = false;
    }
}
