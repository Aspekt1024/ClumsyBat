using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class BaseNodeOld : ScriptableObject {

    public Rect windowRect;
    public bool hasInputs = false;
    public string windowTitle = string.Empty;

    public virtual void DrawWindow()
    {
        windowTitle = EditorGUILayout.TextField("Title", windowTitle);
    }

    public abstract void DrawCurves();

    public virtual void SetInput(BaseInputNode input, Vector2 clickPos) { }

    public virtual void NodeDeleted(BaseNodeOld node) { }

    public virtual BaseInputNode ClickedOnInput(Vector2 pos)
    {
        return null;
    }
}
