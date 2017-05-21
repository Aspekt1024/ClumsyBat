using UnityEngine;
using UnityEditor;

public static class NodeGUI {

    public const float GridSpacing = 10f;

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

    public static string TextField(Rect rect, string value, string label, float xSplitPercent = 0.4f)
    {
        Vector2 pos = rect.position * GridSpacing;
        Vector2 size = rect.size * GridSpacing;

        GUI.Label(new Rect(pos, new Vector2(size.x * xSplitPercent, size.y)), label);
        value = EditorGUI.TextField(new Rect(pos + new Vector2(xSplitPercent * size.x, 0f), new Vector2((1 - xSplitPercent) * size.x, size.y)), value);

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

    #region Node GUI Events

    public delegate void NodeGUIEvents();

    public delegate void NodeGUIPositionEvents(Vector2 delta);
    public static NodeGUIPositionEvents OnMoveAllSelectedNodes;

    public static void MoveAllSelectedNodes(Vector2 delta)
    {
        if (OnMoveAllSelectedNodes != null)
            OnMoveAllSelectedNodes(delta);
    }

    #endregion
}
