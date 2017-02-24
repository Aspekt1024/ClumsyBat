using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BaseNode : ScriptableObject {

    public Rect WindowRect;
    public string WindowTitle = "Untitled";

    protected float width = 200;
    protected float height = 100;

    public virtual void DrawWindow()
    {
        WindowTitle = EditorGUILayout.TextField("Title", WindowTitle);
    }
    
    public virtual void SetWindowRect(Vector2 mousePos)
    {
        WindowRect = new Rect(mousePos.x, mousePos.y, width, height);
    }
}
