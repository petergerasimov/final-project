using UnityEngine;

public class FreezingEffect : MonoBehaviour
{
    [SerializeField]
    private float m_freezeTime = 300f;

    [SerializeField]
    private float m_pulseSpeed = 2.5f;

    private float m_timeRemaining;
    private FreezingPostProcess m_postProcess;
    private DeathScreen m_deathScreen;
    private bool m_isFrozen;
    private GUIStyle m_timerStyle;

    private void Start()
    {
        m_timeRemaining = m_freezeTime;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            m_postProcess = mainCamera.gameObject.AddComponent<FreezingPostProcess>();
            m_postProcess.enabled = this.enabled;
        }
    }

    private void OnEnable()
    {
        if (m_postProcess != null)
        {
            m_postProcess.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (m_postProcess != null)
        {
            m_postProcess.enabled = false;
        }
    }

    private void Update()
    {
        if (m_isFrozen) return;

        m_timeRemaining -= Time.deltaTime;
        if (m_timeRemaining <= 0f)
        {
            m_timeRemaining = 0f;
            m_isFrozen = true;
            if (m_deathScreen != null) m_deathScreen.Show();
            return;
        }

        if (m_postProcess != null && m_postProcess.Material != null)
        {
            m_postProcess.Material.SetFloat("_PulsePhase", Time.time * m_pulseSpeed);
        }
    }

    private void OnGUI()
    {
        if (m_timerStyle == null)
        {
            m_timerStyle = new GUIStyle(GUI.skin.label);
            m_timerStyle.fontSize = 28;
            m_timerStyle.fontStyle = FontStyle.Bold;
            m_timerStyle.normal.textColor = Color.white;
            m_timerStyle.alignment = TextAnchor.LowerRight;
        }

        int minutes = Mathf.FloorToInt(m_timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(m_timeRemaining % 60f);
        string timerText = string.Format("{0}:{1:00}", minutes, seconds);

        float width = 160f;
        float height = 50f;
        float padding = 20f;
        Rect timerRect = new Rect(Screen.width - width - padding, Screen.height - height - padding, width, height);

        GUI.depth = -1000;
        GUI.Label(timerRect, timerText, m_timerStyle);
    }

    private void OnDestroy()
    {
        if (m_postProcess != null) Destroy(m_postProcess);
    }
}
