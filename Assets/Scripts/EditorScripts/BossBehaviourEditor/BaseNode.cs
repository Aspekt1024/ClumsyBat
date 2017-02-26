using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {

    public Rect WindowRect;
    public string WindowTitle = "Untitled";

    protected float width = 200;
    protected float height = 100;

    public readonly List<float> inputs = new List<float>();
    public readonly List<float> outputs = new List<float>();

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
        float outputPos = 0.5f;
        outputs.Add(outputPos);
    }

    protected void AddInput()
    {
        float inputPos = 0.5f;
        inputs.Add(inputPos);
    }

    protected void DrawInterfaces()
    {
        foreach (float output in outputs)
        {
            Handles.color = new Color(0, 0, 1);
            Handles.DrawSolidDisc(new Vector3(width - 7, height * output, 0), Vector3.back, 6f);
            Handles.color = new Color(1, 1, 1);
            Handles.DrawSolidDisc(new Vector3(width - 7, height * output, 0), Vector3.back, 4f);
        }

        foreach (float input in inputs)
        {
            Handles.color = new Color(0, 0, 1);
            Handles.DrawSolidDisc(new Vector3(7f, height * input, 0), Vector3.back, 6f);
            Handles.color = new Color(1, 1, 1);
            Handles.DrawSolidDisc(new Vector3(7f, height * input, 0), Vector3.back, 4f);
        }
    }

    public bool OutputClicked(Vector2 mousePos)
    {
        bool bOutputClicked = false;
        foreach (float output in outputs)
        {
            Vector2 delta = new Vector2 (WindowRect.x + width - 7f, WindowRect.y + height * output) - mousePos;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
            if (dist < 6)
            {
                selectedOutputPos = new Vector2 (width - 7f, height * output);
                bOutputClicked = true;
                break;
            }
        }
        return bOutputClicked;
    }

    public bool ReleasedOnInput(Vector2 mousePos)
    {
        bool bReleasedOnInput = false;
        foreach (float input in inputs)
        {
            Vector2 delta = new Vector2(WindowRect.x + 7f, WindowRect.y + height * input) - mousePos;
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
                x = 7f + WindowRect.x,
                y = height * inputs[0] + WindowRect.y,
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

    public abstract void Tick(float DeltaTime);
}
