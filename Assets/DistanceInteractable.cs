using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Renderer))]
public class DistanceInteractable : MonoBehaviour
{
    public float DisplayDistance = 4f;
    public KeyCode PromptKey = KeyCode.E;
    public Vector3 Offset = Vector3.zero;
    public string TargetTag = "Player";
    public MonoBehaviour ScriptToToggle;
    public AudioClip InteractSound;

    protected Transform m_playerTransform;
    protected Camera m_mainCamera;
    protected Renderer m_renderer;

    protected virtual void Start()
    {
        m_mainCamera = Camera.main;
        GameObject[] players = GameObject.FindGameObjectsWithTag(TargetTag);
        foreach (GameObject player in players)
        {
            if (player.activeSelf == false) continue;
            m_playerTransform = player.transform;
            break;
        }
        m_renderer = GetComponent<Renderer>();
    }

    protected virtual void Update()
    {
        if (m_playerTransform == null) return;

        Vector3 targetPosition = m_renderer.bounds.center;

        float distance = Vector3.Distance(targetPosition, m_playerTransform.position);
        if (distance > DisplayDistance) return;

        if (Input.GetKeyDown(PromptKey)) {
            if (ScriptToToggle != null) ScriptToToggle.enabled = !ScriptToToggle.enabled;
            if (InteractSound != null) AudioManager.Instance.PlaySound(InteractSound);
            OnInteract();
        }
    }

    public virtual void OnInteract()
    {
    }
    protected virtual void OnGUI()
    {
        if (m_playerTransform == null || m_mainCamera == null) return;

        Vector3 targetPosition = m_renderer.bounds.center;

        float distance = Vector3.Distance(targetPosition, m_playerTransform.position);
        if (distance > DisplayDistance) return;

        Vector3 screenPos = m_mainCamera.WorldToScreenPoint(targetPosition + Offset);

        if (screenPos.z <= 0) return;

        float boxWidth = 40f;
        float boxHeight = 40f;

        Rect rect = new Rect(screenPos.x - (boxWidth / 2), Screen.height - screenPos.y - (boxHeight / 2), boxWidth, boxHeight);

        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.alignment = TextAnchor.MiddleCenter;
        boxStyle.fontSize = 20;
        boxStyle.normal.textColor = Color.white;

        Color oldColor = GUI.color;
        GUI.color = new Color(0, 0, 0, 0.8f);
        GUI.Box(rect, GUIContent.none);

        GUI.color = oldColor;
        GUI.Box(rect, PromptKey.ToString(), boxStyle);
    }
}
