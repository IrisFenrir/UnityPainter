using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Painter : MonoBehaviour
{
    public Image paper;
    public ColorSelector colorSelector;

    [Header("Tools")]
    public Button lastStep;
    public Button nextStep;
    public Button fill;
    public Button pencil;
    public Texture2D fillImage;
    public Texture2D pencilImage;

    private int m_width;
    private int m_height;
    private Texture2D m_texture;
    private Color32[] m_colors;

    private Vector3 m_origin;
    private bool m_isFilling;

    private int m_pencilSize = 5;
    private bool m_isPainting;
    private int m_lastMouseX;
    private int m_lastMouseY;
    private float m_pixelWidth;
    private float m_pixelHeight;

    private void Start()
    {
        Vector2 size = paper.GetComponent<RectTransform>().sizeDelta;
        m_width = (int)size.x;
        m_height = (int)size.y;
        m_pixelWidth = size.x / m_width;
        m_pixelHeight = size.y / m_height;
        m_texture = new Texture2D(m_width, m_height);
        paper.sprite = Sprite.Create(m_texture, new Rect(0, 0, m_width, m_height), new Vector2(0.5f, 0.5f));
        m_colors = m_texture.GetPixels32();

        Vector3 scale = paper.rectTransform.lossyScale;
        Vector2 pivot = paper.rectTransform.pivot;
        Vector3 position = paper.transform.position;
        m_origin = position - Vector3.right * pivot.x * size.x * scale.x - Vector3.up * pivot.y * size.y * scale.y;
        new GameObject().transform.position = m_origin;

        fill.onClick.AddListener(OnFillButtonPressed);
        pencil.onClick.AddListener(OnPencilButtonDown);
    }

    private void Update()
    {
        if (m_isFilling && Input.GetMouseButtonDown(0))
        {
            Vector3 delta = Input.mousePosition - m_origin;
            int x = (int)delta.x;
            int y = (int)delta.y;
            if (x >= 0 && x < m_height && y >= 0 && y < m_width)
            {
                Fill(x, y, colorSelector.Color);
                m_isFilling = false;
                Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
            }
        }

        if (m_isPainting)
        {
            Vector3 delta = Input.mousePosition - m_origin;
            int x = (int)(delta.x / m_pixelWidth);
            int y = (int)(delta.y / m_pixelHeight);
            if (x >= 0 && x < m_width && y >= 0 && y < m_height)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    m_lastMouseX = x;
                    m_lastMouseY = y;
                }
                if (Input.GetMouseButton(0))
                {
                    DrawLine(m_lastMouseX, m_lastMouseY, x, y, colorSelector.Color);
                    m_lastMouseX = x;
                    m_lastMouseY = y;
                }
            }
        }
    }


    private void OnFillButtonPressed()
    {
        
        m_isFilling = true;
        Cursor.SetCursor(fillImage, new Vector2(0, 0), CursorMode.Auto);
    }
    private void Fill(int x, int y, Color32 color)
    {
        int pos = y * m_width + x;
        if (pos < 0 || pos >= m_colors.Length || m_colors[pos].Equals(color)) return;
        Stack<int> stack = new Stack<int>();
        Color32 target = m_colors[pos];
        m_colors[pos] = color;
        stack.Push(pos);
        while (stack.Count > 0)
        {
            pos = stack.Pop();
            int i = pos % m_width;
            int j = pos / m_width;
            int neighbor = pos - m_width;
            if (j > 0 && m_colors[neighbor].Equals(target))
            {
                m_colors[neighbor] = color;
                stack.Push(neighbor);
            }
            neighbor = pos + m_width;
            if (j < m_height - 1 && m_colors[neighbor].Equals(target))
            {
                m_colors[neighbor] = color;
                stack.Push(neighbor);
            }
            neighbor = pos - 1;
            if (i > 0 && m_colors[neighbor].Equals(target))
            {
                m_colors[neighbor] = color;
                stack.Push(neighbor);
            }
            neighbor = pos + 1;
            if (i < m_width - 1 && m_colors[neighbor].Equals(target))
            {
                m_colors[neighbor] = color;
                stack.Push(neighbor);
            }
        }
        m_texture.SetPixels32(m_colors);
        m_texture.Apply();
    }

    private void OnPencilButtonDown()
    {
        m_isPainting = !m_isPainting;
        Cursor.SetCursor(m_isPainting ? pencilImage : null, new Vector2(0, pencilImage.height), CursorMode.Auto);
    }
    private void DrawLine(int x1, int y1, int x2, int y2, Color32 color)
    {
        float step = 1f / Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        for (float t = 0; t <= 1f; t += step)
        {
            DrawPoint(Mathf.RoundToInt(Mathf.Lerp(x1, x2, t)), Mathf.RoundToInt(Mathf.Lerp(y1, y2, t)), m_pencilSize, color, false);
        }
        m_texture.SetPixels32(m_colors);
        m_texture.Apply();
    }
    private void DrawPoint(int x, int y, int radius, Color32 color, bool apply = true)
    {
        int xMin = Mathf.Max(x - radius, 0);
        int xMax = Mathf.Min(x + radius, m_width - 1);
        int yMin = Mathf.Max(y - radius, 0);
        int yMax = Mathf.Min(y + radius, m_height - 1);
        for (int i = xMin; i <= xMax; i++)
        {
            for (int j = yMin; j <= yMax; j++)
            {
                if ((i - x) * (i - x) + (j - y) * (j - y) <= radius * radius)
                {
                    m_colors[j * m_width + i] = color;
                }
            }
        }
        if (apply)
        {
            m_texture.SetPixels32(m_colors);
            m_texture.Apply();
        }
    }
}
