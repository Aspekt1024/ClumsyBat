using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BaseNode : ScriptableObject {

    public Rect WindowRect;
    public string WindowTitle = "Untitled";

    protected float width = 200;
    protected float height = 100;

    public readonly List<Vector2> inputs = new List<Vector2>();
    public readonly List<Vector2> outputs = new List<Vector2>();

    private Vector2 selectedOutputPos;
    private BaseNode connectedNode;

    public virtual void DrawWindow()
    {
        WindowTitle = EditorGUILayout.TextField("Title", WindowTitle);
    }
    
    public virtual void SetWindowRect(Vector2 mousePos)
    {
        WindowRect = new Rect(mousePos.x, mousePos.y, width, height);
    }

    protected void AddOutput()
    {
        Vector2 outputPos = new Vector2(width - 7, height / 2);
        outputs.Add(outputPos);

    }

    protected void AddInput()
    {
        Vector2 inputPos = new Vector2(7, height / 2);
        inputs.Add(inputPos);
    }

    protected void DrawOutput()
    {
        foreach (Vector2 output in outputs)
        {
            Handles.color = new Color(0, 0, 1);
            Handles.DrawSolidDisc(new Vector3(output.x, output.y, 0), Vector3.back, 6f);
            Handles.color = new Color(1, 1, 1);
            Handles.DrawSolidDisc(new Vector3(output.x, output.y, 0), Vector3.back, 4f);
        }

        foreach (Vector2 input in inputs)
        {
            Handles.color = new Color(0, 0, 1);
            Handles.DrawSolidDisc(new Vector3(input.x, input.y, 0), Vector3.back, 6f);
            Handles.color = new Color(1, 1, 1);
            Handles.DrawSolidDisc(new Vector3(input.x, input.y, 0), Vector3.back, 4f);
        }
    }

    public bool OutputClicked(Vector2 mousePos)
    {
        bool bOutputClicked = false;
        foreach (Vector2 output in outputs)
        {
            Vector2 delta = output + new Vector2 (WindowRect.x, WindowRect.y) - mousePos;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
            if (dist < 6)
            {
                selectedOutputPos = output;
                bOutputClicked = true;
                break;
            }
        }
        return bOutputClicked;
    }

    public bool ReleasedOnInput(Vector2 mousePos)
    {
        bool bReleasedOnInput = false;
        foreach (Vector2 input in inputs)
        {
            Vector2 delta = input + new Vector2(WindowRect.x, WindowRect.y) - mousePos;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
            if (dist < 10)
            {
                bReleasedOnInput = true;
                break;
            }
        }
        return bReleasedOnInput; // TODO may need to return vector2 instead;
    }
    
    public void DrawCurves()
    {
        // TODO select inputs
        if (connectedNode)
        {
            Rect start = new Rect()
            {
                x = connectedNode.GetSelectedOutputPos().x,
                y = connectedNode.GetSelectedOutputPos().y,
                width = 1,
                height = 1
            };
            Rect end = new Rect()
            {
                x = inputs[0].x + WindowRect.x,
                y = inputs[0].y + WindowRect.y,
                width = 1,
                height = 1
            };
            BossEditor.DrawCurve(start, end);
        }
    }

    public Vector2 GetSelectedOutputPos()
    {
        return selectedOutputPos + new Vector2(WindowRect.x, WindowRect.y);
    }

    public void SetInput(BaseNode outputNode)
    {
        connectedNode = outputNode;
    }
}
