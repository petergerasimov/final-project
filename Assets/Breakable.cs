using UnityEngine;

public class Breakable : MonoBehaviour
{
    public string playerTag = "Player";
    public AudioClip breakSound;
    public AudioClip jumpSound;
    public int requiredJumps = 3;

    private int jumpCount = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag)) return;
        if (collision.transform.position.y <= transform.position.y) return;

        jumpCount++;
        if (jumpCount < requiredJumps) 
        {
            if (jumpSound != null) AudioManager.Instance.PlaySound(jumpSound);
            return;
        }
        
        if (breakSound != null) AudioManager.Instance.PlaySound(breakSound);
        gameObject.SetActive(false);
    }
}
