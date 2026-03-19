using UnityEngine;

public class Breakable : MonoBehaviour
{
    public string PlayerTag = "Player";
    public AudioClip BreakSound;
    public AudioClip JumpSound;
    public int RequiredJumps = 3;

    private int m_jumpCount;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(PlayerTag)) return;
        if (collision.transform.position.y <= transform.position.y) return;

        m_jumpCount++;
        if (m_jumpCount < RequiredJumps) 
        {
            if (JumpSound != null) AudioManager.Instance.PlaySound(JumpSound);
            return;
        }
        
        if (BreakSound != null) AudioManager.Instance.PlaySound(BreakSound);
        gameObject.SetActive(false);
    }
}
