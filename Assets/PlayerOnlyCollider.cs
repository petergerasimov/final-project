using UnityEngine;

public class PlayerOnlyCollider : MonoBehaviour
{
    [SerializeField] private string m_playerTag = "Player";

    private Collider m_collider;
    private GameObject m_player;

    private void Awake()
    {
        m_collider = GetComponent<Collider>();

        GameObject[] players = GameObject.FindGameObjectsWithTag(m_playerTag);
        foreach (GameObject player in players)
        {
            if (!player.activeSelf) continue;

            m_player = player;
            break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == m_player) return;

        Physics.IgnoreCollision(m_collider, collision.collider, true);
    }
}
