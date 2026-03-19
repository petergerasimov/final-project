using UnityEngine;

public class PlayerOnlyCollider : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private Collider _collider;
    private GameObject _player;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        foreach (GameObject player in players)
        {
            if (!player.activeSelf) continue;

            _player = player;
            break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == _player) return;

        Physics.IgnoreCollision(_collider, collision.collider, true);
    }
}
