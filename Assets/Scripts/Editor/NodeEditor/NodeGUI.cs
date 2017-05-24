using UnityEngine;
using UnityEditor;

public class NodeGUI {

    public const float GridSpacing = 10f;
    private const float hPadding = 15f;
    private const float vPadding = 20f;
    
    private int item = 0;
    private Vector2 winSize;

    public static void SetWindow(Vector2 windowSize)
    {
        Instance.item = 0;
        Instance.winSize = windowSize;
    }

    public static void Space(int numSpaces = 1)
    {
        Instance.item += numSpaces;
    }

    public static float VerticalSlider(Rect rect, float value, float minValue, float maxValue, float step, string label)
    {
        Vector2 pos = rect.position * GridSpacing;

        GUI.Label(new Rect(pos + new Vector2(15f, 0f), new Vector2(50, 20)), label);
        value = EditorGUI.FloatField(new Rect(pos + new Vector2(15f, 20f), new Vector2(35f, 15f)), value);

        value = GUI.VerticalSlider(new Rect(pos, rect.size * GridSpacing), value, maxValue, minValue);
        value -= value % step;
        value = Mathf.Clamp(value, minValue, maxValue);
        
        return value;
    }

    public static string TextFieldLayout(string value, string label, float xSplitPercent = 0.4f)
    {
        Rect rect = new Rect(GetPosition(), GetSize());
        Instance.item++;
        return TextField(rect, value, label, xSplitPercent);
    }

    public static string TextField(Rect rect, string value, string label, float xSplitPercent = 0.4f)
    {
        GUI.Label(new Rect(rect.position, new Vector2(rect.size.x * xSplitPercent, rect.size.y)), label);
        value = EditorGUI.TextField(new Rect(rect.position + new Vector2(xSplitPercent * rect.size.x, 0f), new Vector2((1 - xSplitPercent) * rect.size.x, rect.size.y)), value);

        return value;
    }

    public static bool Button(Rect rect, string label)
    {
        Vector2 pos = rect.position * GridSpacing;
        Vector2 size = rect.size * GridSpacing;
        return GUI.Button(new Rect(pos, size), label);
    }
	
    public static float FloatField(Vector2 position, float value, string label)
    {
        GUI.skin.label.alignment = TextAnchor.UpperRight;
        GUI.Label(new Rect(position, new Vector2(50, 20)), label);
        return EditorGUI.FloatField(new Rect(position + new Vector2(53, 1), new Vector2(40, 15)), value);
    }

    private static Vector2 GetPosition()
    {
        return new Vector2(hPadding, vPadding + Instance.item * 20f);
    }

    private static Vector2 GetSize()
    {
        return new Vector2(Instance.winSize.x - hPadding * 2f, 18f);
    }

    #region Node GUI Events

    public delegate void NodeGUIEvents();

    public delegate void NodeGUIPositionEvents(Vector2 delta);
    public static NodeGUIPositionEvents OnMoveAllSelectedNodes;

    public static void MoveAllSelectedNodes(Vector2 delta)
    {
        Debug.Log("still a thing");
        if (OnMoveAllSelectedNodes != null)
            OnMoveAllSelectedNodes(delta);
    }

    #endregion
    
    #region singleton existence

    private static NodeGUI _instance;
    private static bool _appQuitting = false;
    private static object _lock = new object();

    public static NodeGUI Instance
    {
        get
        {
            if (_appQuitting) return null;

            lock (_lock)
            {
                if (_instance == null)
                    _instance = new NodeGUI();

                return _instance;
            }
        }
    }

    ~NodeGUI()
    {
        _appQuitting = true;
    }
    #endregion
}
