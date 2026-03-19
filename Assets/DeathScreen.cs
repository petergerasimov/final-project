using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    private bool m_isShowing;
    private Texture2D m_backgroundTexture;
    private Texture2D m_buttonNormalTexture;
    private Texture2D m_buttonHoverTexture;

    private void Start()
    {
        m_backgroundTexture = new Texture2D(1, 1);
        m_backgroundTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.85f));
        m_backgroundTexture.Apply();

        m_buttonNormalTexture = MakeOutlineTexture(200, 50, Color.white, Color.clear);
        m_buttonHoverTexture = MakeOutlineTexture(200, 50, Color.yellow, Color.clear);
    }

    public void Show()
    {
        m_isShowing = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnGUI()
    {
        if (!m_isShowing) return;

        GUI.depth = -3000;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_backgroundTexture);

        float buttonWidth = 200f;
        float buttonHeight = 50f;
        float spacing = 20f;

        float titleHeight = 60f;
        float totalHeight = titleHeight + spacing + buttonHeight;
        float startY = (Screen.height - totalHeight) / 2f;
        float startX = (Screen.width - buttonWidth) / 2f;

        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 36;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        GUI.Label(new Rect(startX - 50f, startY, buttonWidth + 100f, titleHeight), "YOU FROZE", titleStyle);

        float buttonsStartY = startY + titleHeight + spacing;
        Rect exitRect = new Rect(startX, buttonsStartY, buttonWidth, buttonHeight);

        DrawMenuButton(exitRect, "EXIT", () => Application.Quit());
    }

    private void DrawMenuButton(Rect rect, string text, System.Action onClick)
    {
        bool isHovered = rect.Contains(Event.current.mousePosition);

        GUIStyle style = new GUIStyle();
        style.normal.background = isHovered ? m_buttonHoverTexture : m_buttonNormalTexture;
        style.normal.textColor = Color.white;
        style.fontSize = 22;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        if (GUI.Button(rect, text, style))
        {
            onClick?.Invoke();
        }
    }

    private Texture2D MakeOutlineTexture(int width, int height, Color borderColor, Color fillColor)
    {
        Texture2D texture = new Texture2D(width, height);
        int border = 2;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < border || x >= width - border || y < border || y >= height - border)
                {
                    texture.SetPixel(x, y, borderColor);
                }
                else
                {
                    texture.SetPixel(x, y, fillColor);
                }
            }
        }
        texture.Apply();
        return texture;
    }

    public bool IsShowing { get { return m_isShowing; } }

    private void OnDestroy()
    {
        if (m_backgroundTexture != null) Destroy(m_backgroundTexture);
        if (m_buttonNormalTexture != null) Destroy(m_buttonNormalTexture);
        if (m_buttonHoverTexture != null) Destroy(m_buttonHoverTexture);
    }
}
