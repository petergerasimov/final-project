using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource m_fakeAudioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        m_fakeAudioSource = GetComponent<AudioSource>();
        m_fakeAudioSource.playOnAwake = false;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        m_fakeAudioSource.PlayOneShot(clip);
    }
}
