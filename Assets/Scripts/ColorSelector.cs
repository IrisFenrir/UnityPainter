using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    public Color Color => m_color.Value;

    [Header("Color Picker")]
    public Button colorPickerButton;
    public Image colorArea;

    private Texture2D m_colorPickerTexture;
    private bool m_isPicking;

    [Header("Color Circle")]
    public Image colorCircle;
    public Transform HPointer;
    public Transform SVPointer;

    private Transform m_colorCircleCenter;
    private Material m_colorCircleMat;
    private float m_outerRadiusX;
    private float m_outerRadiusY;
    private float m_innerRadiusX;
    private float m_innerRadiusY;
    private bool m_isHPointerMoving;
    private float m_halfSideX;
    private float m_halfSideY;
    private bool m_isSVPointerMoving;

    [Header("HSV Page")]
    public GameObject HSVPage;
    public Slider HSlider;
    public InputField HText;
    public Slider SSlider;
    public InputField SText;
    public Material SMat;
    public Slider VSlider;
    public InputField VText;
    public Material VMat;

    [Header("RGB Page")]
    public GameObject RGBPage;
    public Slider RSlider;
    public InputField RText;
    public Material RMat;
    public Slider GSlider;
    public InputField GText;
    public Material GMat;
    public Slider BSlider;
    public InputField BText;
    public Material BMat;

    [Header("Other")]
    public Dropdown colorType;
    public Slider ASlider;
    public InputField AText;
    public Material AMat;
    public InputField HexText;

    private Property<Color> m_color;
    private Property<float> m_H;
    private Property<float> m_S;
    private Property<float> m_V;
    private Property<float> m_A;
    private Property<float> m_R;
    private Property<float> m_G;
    private Property<float> m_B;
    private Property<string> m_Hex;

    private void Start()
    {
        m_color = new Property<Color>();
        m_color.OnValueChanged += val =>
        {
            colorArea.color = val;
            Color.RGBToHSV(val, out float H, out float S, out float V);
            m_H.Value = H;
            m_S.Value = S;
            m_V.Value = V;
            m_A.Value = val.a;
            m_R.Value = val.r;
            m_G.Value = val.g;
            m_B.Value = val.b;
            m_Hex.Value = ColorUtility.ToHtmlStringRGBA(val);
            AMat.SetColor("_Color1", new Color(val.r, val.g, val.b, 0));
            AMat.SetColor("_Color2", new Color(val.r, val.g, val.b, 1));
            RMat.SetColor("_Color1", new Color(0, val.g, val.b));
            RMat.SetColor("_Color2", new Color(1, val.g, val.b));
            GMat.SetColor("_Color1", new Color(val.r, 0, val.b));
            GMat.SetColor("_Color2", new Color(val.r, 1, val.b));
            BMat.SetColor("_Color1", new Color(val.r, val.g, 0));
            BMat.SetColor("_Color2", new Color(val.r, val.g, 1));
        };

        m_H = new Property<float>();
        m_H.OnValueChanged += val =>
        {
            m_color.Value = Color.HSVToRGB(m_H.Value, m_S.Value, m_V.Value);
            SetHPointerPosition(val * 2 * Mathf.PI);
            m_colorCircleMat.SetFloat("_Hue", val * 360f);
            HSlider.value = val * 360f;
            HText.text = (val * 360f).ToString();
            SMat.SetColor("_Color1", Color.HSVToRGB(m_H.Value, 0, m_V.Value));
            SMat.SetColor("_Color2", Color.HSVToRGB(m_H.Value, 1, m_V.Value));
            VMat.SetColor("_Color1", Color.HSVToRGB(m_H.Value, m_S.Value, 0));
            VMat.SetColor("_Color2", Color.HSVToRGB(m_H.Value, m_S.Value, 1));
        };

        m_S = new Property<float>();
        m_S.OnValueChanged += val =>
        {
            m_color.Value = Color.HSVToRGB(m_H.Value, val, m_V.Value);
            SetSVPointerPosition(val * 2 * m_halfSideX - m_halfSideX, m_V.Value * 2 * m_halfSideY - m_halfSideY);
            SSlider.value = val * 100f;
            SText.text = (val * 100f).ToString();
            VMat.SetColor("_Color1", Color.HSVToRGB(m_H.Value, val, 0));
            VMat.SetColor("_Color2", Color.HSVToRGB(m_H.Value, val, 1));
        };

        m_V = new Property<float>();
        m_V.OnValueChanged += val =>
        {
            m_color.Value = Color.HSVToRGB(m_H.Value, m_S.Value, val);
            SetSVPointerPosition(m_S.Value * 2 * m_halfSideX - m_halfSideX, val * 2 * m_halfSideY - m_halfSideY);
            VSlider.value = val * 100f;
            VText.text = (val * 100f).ToString();
            SMat.SetColor("_Color1", Color.HSVToRGB(m_H.Value, 0, val));
            SMat.SetColor("_Color2", Color.HSVToRGB(m_H.Value, 1, val));
        };

        m_A = new Property<float>();
        m_A.OnValueChanged += val =>
        {
            Color color = m_color.Value;
            color.a = val;
            m_color.Value = color;
            ASlider.value = val * 100f;
            AText.text = (val * 100f).ToString();
        };

        m_R = new Property<float>();
        m_R.OnValueChanged += val =>
        {
            m_color.Value = new Color(val, m_G.Value, m_B.Value);
            RSlider.value = val * 255f;
            RText.text = (val * 255f).ToString();
        };

        m_G = new Property<float>();
        m_G.OnValueChanged += val =>
        {
            m_color.Value = new Color(m_R.Value, val, m_B.Value);
            GSlider.value = val * 255f;
            GText.text = (val * 255f).ToString();
        };

        m_B = new Property<float>();
        m_B.OnValueChanged += val =>
        {
            m_color.Value = new Color(m_R.Value, m_G.Value, val);
            BSlider.value = val * 255f;
            BText.text = (val * 255f).ToString();
        };

        m_Hex = new Property<string>(string.Empty);
        m_Hex.OnValueChanged += val =>
        {
            if (!ColorUtility.TryParseHtmlString("#" + val, out Color color)) return;
            m_color.Value = color;
            HexText.text = val;
        };

        ColorPickerInit();
        ColorCircleInit();
        HSVPageInit();
        RGBPageInit();
        OtherInit();

        m_color.Value = Color.yellow;
    }

    private void Update()
    {
        ColorPickerUpdate();
        ColorCircleUpdate();
    }

    private void ColorPickerInit()
    {
        colorPickerButton.onClick.AddListener(() =>
        {
            StartCoroutine(CaptureScreen());
        });
    }
    private IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();
        m_colorPickerTexture = new Texture2D(Screen.width, Screen.height);
        m_colorPickerTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        m_colorPickerTexture.Apply();
        m_isPicking = true;
    }
    private void ColorPickerUpdate()
    {
        if (!m_isPicking || m_colorPickerTexture == null) return;

        int x = (int)Input.mousePosition.x;
        int y = (int)Input.mousePosition.y;
        Color color = m_colorPickerTexture.GetPixel(x, y);
        colorArea.color = color;

        if (Input.GetMouseButtonDown(0))
        {
            m_isPicking = false;
            m_color.Value = color;
        }
    }

    private void ColorCircleInit()
    {
        RectTransform rectTrans = colorCircle.GetComponent<RectTransform>();
        Vector2 size = rectTrans.sizeDelta;
        Vector3 scale = colorCircle.transform.lossyScale;
        float width = size.x * scale.x;
        float height = size.y * scale.y;
        Vector2 pivot = rectTrans.pivot;
        Vector2 center = colorCircle.transform.position - colorCircle.transform.right * (pivot.x - 0.5f) * width - colorCircle.transform.up * (pivot.y - 0.5f) * height;
        m_colorCircleCenter = new GameObject("ColorCircleCenter").transform;
        m_colorCircleCenter.parent = colorCircle.transform;
        m_colorCircleCenter.position = center;
        m_colorCircleCenter.right = colorCircle.transform.right;
        m_colorCircleCenter.up = colorCircle.transform.up;

        m_colorCircleMat = colorCircle.material;
        float outerRadius = m_colorCircleMat.GetFloat("_OuterRadius");
        float innerRadius = m_colorCircleMat.GetFloat("_InnerRadius");
        m_outerRadiusX = width * 0.5f * outerRadius;
        m_outerRadiusY = height * 0.5f * outerRadius;
        m_innerRadiusX = width * 0.5f * innerRadius;
        m_innerRadiusY = height * 0.5f * innerRadius;

        float side = m_colorCircleMat.GetFloat("_CenterSide");
        m_halfSideX = width * side * 0.5f;
        m_halfSideY = height * side * 0.5f;
    }
    private void ColorCircleUpdate()
    {
        Vector3 mousePosition = m_colorCircleCenter.InverseTransformPoint(Input.mousePosition);
        float x = mousePosition.x;
        float y = mousePosition.y;

        if (!m_isHPointerMoving)
        {
            if (Input.GetMouseButtonDown(0) && !IsInsideEllipse(x, y, m_innerRadiusX, m_innerRadiusY) && IsInsideEllipse(x, y, m_outerRadiusX, m_outerRadiusY))
            {
                m_isHPointerMoving = true;
            }
        }
        else
        {
            float angle = Mathf.Atan2(y, x);
            SetHPointerPosition(angle);
            m_H.Value = (angle + 2 * Mathf.PI) % (2 * Mathf.PI) / (2 * Mathf.PI);

            if (Input.GetMouseButtonUp(0))
            {
                m_isHPointerMoving = false;
            }
        }

        if (!m_isSVPointerMoving)
        {
            if (Input.GetMouseButtonDown(0) && IsInsideRectangle(x, y, m_halfSideX, m_halfSideY))
            {
                m_isSVPointerMoving = true;
            }
        }
        else
        {
            x = Mathf.Clamp(x, -m_halfSideX, m_halfSideX);
            y = Mathf.Clamp(y, -m_halfSideY, m_halfSideY);
            SetSVPointerPosition(x, y);
            m_S.Value = x / m_halfSideX / 2 + 0.5f;
            m_V.Value = y / m_halfSideY / 2 + 0.5f;

            if (Input.GetMouseButtonUp(0))
            {
                m_isSVPointerMoving = false;
            }
        }
    }
    private bool IsInsideEllipse(float x, float y, float radiusX, float radiusY)
    {
        return x * x / (radiusX * radiusX) + y * y / (radiusY * radiusY) <= 1;
    }
    private bool IsInsideRectangle(float x, float y, float halfSideX, float halfSideY)
    {
        x /= halfSideX;
        y /= halfSideY;
        return x >= -1 && x <= 1 && y >= -1 && y <= 1;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="angle">»¡¶È</param>
    private void SetHPointerPosition(float angle)
    {
        HPointer.position = m_colorCircleCenter.TransformPoint(
            new Vector3(
                (m_outerRadiusX + m_innerRadiusX) * 0.5f * Mathf.Cos(angle),
                (m_outerRadiusY + m_innerRadiusY) * 0.5f * Mathf.Sin(angle), 0)
            );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">-halfSideX ~ halfSideX</param>
    /// <param name="y">-halfSideY ~ halfSideY</param>
    private void SetSVPointerPosition(float x, float y)
    {
        SVPointer.position = m_colorCircleCenter.TransformPoint(new Vector3(x, y));
    }

    private void HSVPageInit()
    {
        HSlider.onValueChanged.AddListener(val =>
        {
            if (m_isHPointerMoving) return;
            m_H.Value = val / 360f;
        });
        HText.onEndEdit.AddListener(text =>
        {
            if (m_isHPointerMoving) return;
            if (!int.TryParse(text, out int val)) return;
            val = Mathf.Clamp(val, 0, 360);
            m_H.Value = val / 360f;
        });

        SSlider.onValueChanged.AddListener(val =>
        {
            if (m_isSVPointerMoving) return;
            m_S.Value = val / 100f;
        });
        SText.onEndEdit.AddListener(text =>
        {
            if (m_isSVPointerMoving) return;
            if (!int.TryParse(text, out int val)) return;
            val = Mathf.Clamp(val, 0, 100);
            m_S.Value = val / 100f;
        });

        VSlider.onValueChanged.AddListener(val =>
        {
            if (m_isSVPointerMoving) return;
            m_V.Value = val / 100f;
        });
        VText.onEndEdit.AddListener(text =>
        {
            if (m_isSVPointerMoving) return;
            if (!int.TryParse(text, out int val)) return;
            val = Mathf.Clamp(val, 0, 100);
            m_V.Value = val / 100f;
        });
    }

    private void RGBPageInit()
    {
        RSlider.onValueChanged.AddListener(val =>
        {
            m_R.Value = val / 255f;
        });
        RText.onEndEdit.AddListener(text =>
        {
            if (!int.TryParse(text, out int val)) return;
            val = Mathf.Clamp(val, 0, 255);
            m_R.Value = val / 255f;
        });

        GSlider.onValueChanged.AddListener(val =>
        {
            m_G.Value = val / 255f;
        });
        GText.onEndEdit.AddListener(text =>
        {
            if (!int.TryParse(text, out int val)) return;
            val = Mathf.Clamp(val, 0, 255);
            m_G.Value = val / 255f;
        });

        BSlider.onValueChanged.AddListener(val =>
        {
            m_B.Value = val / 255f;
        });
        BText.onEndEdit.AddListener(text =>
        {
            if (!int.TryParse(text, out int val)) return;
            val = Mathf.Clamp(val, 0, 255);
            m_B.Value = val / 255f;
        });
    }

    private void OtherInit()
    {
        ASlider.onValueChanged.AddListener(val =>
        {
            m_A.Value = val / 100f;
        });
        AText.onEndEdit.AddListener(text =>
        {
            if (!int.TryParse(text, out int val)) return;
            val = Mathf.Clamp(val, 0, 100);
            m_A.Value = val / 100f;
        });

        colorType.onValueChanged.AddListener(index =>
        {
            switch(index)
            {
                case 0:
                    HSVPage.SetActive(true);
                    RGBPage.SetActive(false);
                    break;
                case 1:
                    HSVPage.SetActive(false);
                    RGBPage.SetActive(true);
                    break;
            }
        });

        HexText.onEndEdit.AddListener(text =>
        {
            if (!ColorUtility.TryParseHtmlString("#" + text, out Color color)) return;
            m_Hex.Value = text;
        });
    }
}
