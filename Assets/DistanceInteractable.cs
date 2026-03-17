using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DistanceInteractable : MonoBehaviour
{
    public float displayDistance = 4f;
    public KeyCode promptKey = KeyCode.E;
    public Vector3 offset = Vector3.zero;
    public string targetTag = "Player";

    protected Transform _playerTransform;
    protected Camera _mainCamera;
    protected Renderer _renderer;

    protected virtual void Start()
    {
        _mainCamera = Camera.main;
        GameObject[] players = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject player in players)
        {
            if (player.activeSelf == false) continue;
            _playerTransform = player.transform;
            break;
        }
        _renderer = GetComponent<Renderer>();
    }

    protected virtual void Update()
    {
        if (_playerTransform == null) return;

        Vector3 targetPosition = _renderer.bounds.center;

        float distance = Vector3.Distance(targetPosition, _playerTransform.position);
        if (distance > displayDistance) return;

        if (Input.GetKeyDown(promptKey)) OnInteract();
    }

    public virtual void OnInteract()
    {
    }
    protected virtual void OnGUI()
    {
        if (_playerTransform == null || _mainCamera == null) return;

        Vector3 targetPosition = _renderer.bounds.center;

        float distance = Vector3.Distance(targetPosition, _playerTransform.position);
        if (distance > displayDistance) return;

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(targetPosition + offset);

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
        GUI.Box(rect, promptKey.ToString(), boxStyle);
    }
}
