using UnityEngine;

[DefaultExecutionOrder(100)]
public class FreezingToggle : MonoBehaviour
{
    private FreezingEffect m_freezingEffect;
    private MonoBehaviour m_endScreenInteractable;

    private void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (!player.activeSelf) continue;
            m_freezingEffect = player.GetComponent<FreezingEffect>();
            break;
        }
        GameObject door = GameObject.Find("ExitDoor");
        m_endScreenInteractable = door.GetComponent<MonoBehaviour>();
    }

    private void OnEnable()
    {
        if (m_freezingEffect != null) m_freezingEffect.enabled = true;
        if (m_endScreenInteractable != null) m_endScreenInteractable.enabled = false;
    }

    private void OnDisable()
    {
        if (m_freezingEffect != null) m_freezingEffect.enabled = false;
        if (m_endScreenInteractable != null) m_endScreenInteractable.enabled = true;
    }
}
