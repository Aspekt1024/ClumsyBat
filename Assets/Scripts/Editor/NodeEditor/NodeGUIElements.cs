﻿using UnityEngine;
using UnityEditor;

public static class NodeGUIElements {

    public const float GridSpacing = 10f;

    public static float VerticalSlider(BaseNode node, Rect rect, float value, float minValue, float maxValue, float step, string label)
    {
        Vector2 pos = node.WindowRect.position + rect.position * GridSpacing;
        
        GUI.Label(new Rect(pos + new Vector2(15f, 0f), new Vector2(50, 20)), label);
        value = EditorGUI.FloatField(new Rect(pos + new Vector2(15f, 20f), new Vector2(35f, 15f)), value);
        
        value = GUI.VerticalSlider(new Rect(pos, rect.size * GridSpacing), value, maxValue, minValue);
        value -= value % step;
        value = Mathf.Clamp(value, minValue, maxValue);

        return value;
    }
	
}