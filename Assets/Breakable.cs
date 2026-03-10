using UnityEngine;

public class Breakable : MonoBehaviour
{
    public string playerTag = "Player";
    public int requiredJumps = 3;
    public bool onlyCountJumpsFromAbove = true;

    private int jumpCount = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag))
        {
            return;
        }

        if (onlyCountJumpsFromAbove && collision.transform.position.y <= transform.position.y)
        {
            return;
        }
        jumpCount++;
        if (jumpCount >= requiredJumps)
        {
            gameObject.SetActive(false);
        }
    }
}
