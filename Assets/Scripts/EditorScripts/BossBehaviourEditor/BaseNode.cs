using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {

    public Rect WindowRect;
    public string WindowTitle = "Untitled";

    protected float width = 200;
    protected float height = 100;

    public List<InterfaceType> inputs;
    public List<InterfaceType> outputs;

    public struct InterfaceType
    {
        public float yPos;
        public BaseNode connectedNode;
        public int connectedIndex;
    }

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
        InterfaceType outputPos = new InterfaceType()
        {
            yPos = 0.5f,
            connectedNode = null,
            connectedIndex = 0
        };
        outputs.Add(outputPos);
    }

    protected void AddInput()
    {
        InterfaceType inputPos = new InterfaceType()
        {
            yPos = 0.5f,
            connectedNode = null,
            connectedIndex = 0
        };
        inputs.Add(inputPos);
    }

    protected void DrawInterfaces()
    {
        foreach (InterfaceType output in outputs)
        {
            Handles.color = new Color(0, 0, 1);
            Handles.DrawSolidDisc(new Vector3(width - 7, height * output.yPos, 0), Vector3.back, 6f);
            Handles.color = new Color(1, 1, 1);
            Handles.DrawSolidDisc(new Vector3(width - 7, height * output.yPos, 0), Vector3.back, 4f);

            if (output.connectedNode != null)
            {
                Handles.color = new Color(0.4f, 0.4f, 1);
                Handles.DrawSolidDisc(new Vector3(width - 7, height * output.yPos, 0), Vector3.back, 3f);
            }
        }

        foreach (InterfaceType input in inputs)
        {
            Handles.color = new Color(0, 0, 1);
            Handles.DrawSolidDisc(new Vector3(7f, height * input.yPos, 0), Vector3.back, 6f);
            Handles.color = new Color(1, 1, 1);
            Handles.DrawSolidDisc(new Vector3(7f, height * input.yPos, 0), Vector3.back, 4f);

            if (input.connectedNode != null)
            {
                Handles.color = new Color(0.4f, 0.4f, 1);
                Handles.DrawSolidDisc(new Vector3(7f, height * input.yPos, 0), Vector3.back, 3f);
            }
        }
    }

    public int OutputClicked(Vector2 mousePos)
    {
        int chosenOutput = -1;
        for (int i = 0; i < outputs.Count; i++)
        {
            Vector2 delta = new Vector2 (WindowRect.x + width - 7f, WindowRect.y + height * outputs[i].yPos) - mousePos;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
            if (dist < 6)
            {
                selectedOutputPos = new Vector2 (width - 7f, height * outputs[i].yPos);
                chosenOutput = i;
                break;
            }
        }
        return chosenOutput;
    }

    public int InputClicked(Vector2 mousePos)
    {
        int chosenInput = -1;
        for (int i = 0; i < inputs.Count; i++)
        {
            Vector2 delta = new Vector2(WindowRect.x + 7f, WindowRect.y + height * inputs[i].yPos) - mousePos;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
            if (dist < 6)
            {
                chosenInput = i;
                break;
            }
        }
        return chosenInput;
    }

    public int GetReleasedNode(Vector2 mousePos)
    {
        int chosenInput = -1;
        for (int i = 0; i < inputs.Count; i++)
        {
            Vector2 delta = new Vector2(WindowRect.x + 7f, WindowRect.y + height * inputs[i].yPos) - mousePos;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
            if (dist < 10)
            {
                chosenInput = i;
                break;
            }
        }
        return chosenInput;
    }
    
    public void DrawCurves()
    {
        foreach (InterfaceType input in inputs)
        {
            if (input.connectedNode)
            {
                Vector2 outPos = input.connectedNode.GetOutputPos(input.connectedIndex);
                Rect start = new Rect()
                {
                    x = outPos.x,
                    y = outPos.y,
                    width = 1,
                    height = 1
                };
                Rect end = new Rect()
                {
                    x = 7f + WindowRect.x,
                    y = height * input.yPos + WindowRect.y,
                    width = 1,
                    height = 1
                };
                BossEditor.DrawCurve(start, end);
            }
        }
    }

    public Vector2 GetOutputPos(int outputIndex)
    {
        Vector2 outPos = new Vector2(width - 7f, height * outputs[outputIndex].yPos);
        return outPos + new Vector2(WindowRect.x, WindowRect.y);
    }

    public Vector2 GetSelectedOutputPos()
    {
        return selectedOutputPos + new Vector2(WindowRect.x, WindowRect.y);
    }

    public void SetInput(int inputIndex, InterfaceType otherNode)
    {
        if (otherNode.connectedNode == this) return;    // Can't connect to self
        
        var input = inputs[inputIndex];
        if (input.connectedNode != null)
            input.connectedNode.DisconnectOutput(input.connectedIndex);

        input.connectedNode = otherNode.connectedNode;
        input.connectedIndex = otherNode.connectedIndex;
        inputs[inputIndex] = input;
        
        otherNode.connectedNode.SetOutput(otherNode.connectedIndex, input);
    }

    public void SetOutput(int outputIndex, InterfaceType otherNode)
    {
        var output = outputs[outputIndex];
        output.connectedNode = otherNode.connectedNode;
        output.connectedIndex = otherNode.connectedIndex;
        outputs[outputIndex] = output;
    }

    public void DisconnectOutput(int outputIndex)
    {
        var output = outputs[outputIndex];
        output.connectedNode = null;
        outputs[outputIndex] = output;
    }

    public void DisconnectInput(int inputIndex)
    {
        var input = inputs[inputIndex];
        input.connectedNode = null;
        inputs[inputIndex] = input;
    }

    public bool InputIsConnected(int inputIndex)
    {
        return inputs[inputIndex].connectedNode != null;
    }

    public InterfaceType GetConnectedInterfaceFromInput(int inputIndex)
    {
        InterfaceType output = new InterfaceType();
        output.connectedNode = inputs[inputIndex].connectedNode;
        output.connectedIndex = inputs[inputIndex].connectedIndex;
        return output;
    }

    public abstract void Tick(float DeltaTime);
}
