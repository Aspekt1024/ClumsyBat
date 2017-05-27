using System;
using UnityEngine;
using UnityEditor;

public class NodeGUI {

    public const float GridSpacing = 10f;
    public const float RowHeight = 20f;
    private const float hPadding = 15f;
    private const float vPadding = 30f;
    
    private float item = 0;
    private Vector2 winSize;

    public static void SetWindow(Vector2 windowSize)
    {
        Instance.item = 0;
        Instance.winSize = windowSize;
    }

    public static void Space(float space = 1f)
    {
        Instance.item += space;
    }

    // TODO redo this... shouldn't really be using GridSpacing to define window sizing
    public static float VerticalSlider(Rect rect, float value, float minValue, float maxValue, float step, string label)
    {
        Vector2 pos = rect.position * GridSpacing;

        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUI.Label(new Rect(pos + new Vector2(15f, 0f), new Vector2(50, 20)), label);
        value = EditorGUI.FloatField(new Rect(pos + new Vector2(15f, 20f), new Vector2(35f, 15f)), value);

        value = GUI.VerticalSlider(new Rect(pos, rect.size * GridSpacing), value, maxValue, minValue);
        value -= value % step;
        value = Mathf.Clamp(value, minValue, maxValue);
        
        return value;
    }
    
    public static void LabelLayout(string label)
    {
        Label(NextLayoutRect(), label);
    }

    public static void Label(Rect rect, string label)
    {
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUI.Label(rect, label);
    }

    public static string TextFieldLayout(string value, string label, float xSplitPercent = 0.4f)
    {
        return TextField(NextLayoutRect(), value, label, xSplitPercent);
    }

    public static string TextField(Rect rect, string value, string label, float xSplitPercent = 0.4f)
    {
        FieldLabel(rect, label, xSplitPercent);
        return EditorGUI.TextField(FieldRect(rect, xSplitPercent), value);
    }

    public static bool ButtonLayout(string label)
    {
        return Button(NextLayoutRect(), label);
    }

    public static bool Button(Rect rect, string label)
    {
        return GUI.Button(rect, label);
    }

    public static float FloatFieldWithPrefixLayout(float value, string label, float xSplitPercent = 0.6f)
    {
        return FloatFieldWithPrefix(NextLayoutRect(), value, label, xSplitPercent); ;
    }

    public static float FloatFieldWithPrefix(Rect rect, float value, string label, float xSplitPercent = 0.6f)
    {
        value = EditorGUI.FloatField(new Rect(rect.position, new Vector2(rect.size.x * xSplitPercent, rect.size.y)), value);

        Vector2 labelPos = rect.position + new Vector2(rect.size.x * xSplitPercent, 0f);
        Vector2 labelSize = new Vector2(rect.size.x * (1 - xSplitPercent), rect.size.y);

        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUI.Label(new Rect(labelPos, labelSize), label);

        return value;
    }

    public static float FloatFieldLayout(float val)
    {
        return FloatFieldLayout(val, "", 0f);
    }

    public static float FloatFieldLayout(float val, string label, float xSplitPercent = 0.4f)
    {
        return FloatField(NextLayoutRect(), val, label, xSplitPercent);
    }

    public static float FloatField(Rect rect, float val, string label, float xSplitPercent = 0.4f)
    {
        FieldLabel(rect, label, xSplitPercent);
        return EditorGUI.FloatField(FieldRect(rect, xSplitPercent), val);
    }
    
    public static int PopupLayout(string label, int selectedIndex, string[] selections, float xSplitPercent = 0.4f)
    {
        return Popup(NextLayoutRect(), label, selectedIndex, selections, xSplitPercent);
    }

    public static int Popup(Rect rect, string label, int selectedIndex, string[] selections, float xSplitPercent = 0.4f)
    {
        FieldLabel(rect, label, xSplitPercent);
        return EditorGUI.Popup(FieldRect(rect, xSplitPercent), selectedIndex, selections);
    }

    public static Enum EnumPopupLayout(string label, Enum selected, float xSplitPercent = 0.4f)
    {
        return EnumPopup(NextLayoutRect(), label, selected, xSplitPercent);
    }

    public static Enum EnumPopup(Rect rect, string label, Enum selected, float xSplitPercent = 0.4f)
    {
        FieldLabel(rect, label, xSplitPercent);
        return EditorGUI.EnumPopup(FieldRect(rect, xSplitPercent), selected);
    }

    public static int IntFieldLayout(string label, int val, float xSplitPercent = 0.4f)
    {
        return IntField(NextLayoutRect(), label, val, xSplitPercent);
    }

    public static int IntField(Rect rect, string label, int val, float xSplitPercent = 0.4f)
    {
        FieldLabel(rect, label, xSplitPercent);
        return EditorGUI.IntField(FieldRect(rect, xSplitPercent), val);
    }

    public static bool ToggleLayout(string label, bool val, float xSplitPercent = 0.7f)
    {
        return Toggle(NextLayoutRect(), label, val);
    }

    public static bool Toggle(Rect rect, string label, bool val, float xSplitPercent = 0.7f)
    {
        FieldLabel(rect, label, xSplitPercent);
        return EditorGUI.Toggle(FieldRect(rect, xSplitPercent), val);
    }

    #region layout
    private static void FieldLabel(Rect rect, string label, float xSplitPercent = 0.4f)
    {
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUI.Label(new Rect(rect.position, new Vector2(rect.size.x * xSplitPercent, rect.size.y)), label);
    }

    private static Rect FieldRect(Rect rect, float xSplitPercent)
    {
        Vector2 fieldPos = rect.position + new Vector2(rect.size.x * xSplitPercent, 0f);
        Vector2 fieldSize = new Vector2(rect.size.x * (1 - xSplitPercent), rect.size.y);
        return new Rect(fieldPos, fieldSize);
    }

    private static Rect NextLayoutRect()
    {
        Rect rect = GetLayoutRect();
        Instance.item += 1f;
        return rect;
    }
    
    private static Rect GetLayoutRect()
    {
        return new Rect(GetPosition(), GetSize());
    }

    public static Vector2 GetPosition()
    {
        return new Vector2(hPadding, vPadding + Instance.item * RowHeight);
    }

    private static Vector2 GetSize()
    {
        return new Vector2(Instance.winSize.x - hPadding * 2f, RowHeight);
    }
    #endregion

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
