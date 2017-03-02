using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {

    public Rect WindowRect;
    public string WindowTitle = "Untitled";

    public List<InterfaceType> inputs = new List<InterfaceType>();
    public List<InterfaceType> outputs = new List<InterfaceType>();

    [System.Serializable]
    public struct InterfaceType
    {
        public float yPos;
        public BaseNode connectedNode;
        public int interfaceIndex;
        public int identifier;
        public string label;
    }

    private Vector2 selectedOutputPos;

    public abstract void SetupNode();

    public virtual void DrawWindow()
    {
        WindowTitle = EditorGUILayout.TextField("Title", WindowTitle);
        DrawInterfaces();
    }
    
    public virtual void SetWindowRect(Vector2 position)
    {
        WindowRect = new Rect(position.x, position.y, WindowRect.width, WindowRect.height);
    }

    protected void AddOutput(int id = 0)
    {
        InterfaceType outputPos = new InterfaceType()
        {
            yPos = 0,
            connectedNode = null,
            interfaceIndex = 0,
            identifier = id,
            label = ""
        };
        outputs.Add(outputPos);
    }

    protected void AddInput(int id = 0)
    {
        InterfaceType inputPos = new InterfaceType()
        {
            yPos = 0,
            connectedNode = null,
            interfaceIndex = 0,
            identifier = id,
            label = ""
        };
        inputs.Add(inputPos);
    }

    protected void DrawInterfaces()
    {
        foreach (InterfaceType output in outputs)
        {
            Vector3 position = new Vector3(WindowRect.width - 7f, output.yPos, 0f);
            DrawInterfaceAt(position, output.connectedNode != null);
            if (output.label != string.Empty)
            {
                EditorGUIUtility.labelWidth = 70f;
                var gs = GUI.skin.GetStyle("Label");
                gs.alignment = TextAnchor.UpperRight;
                EditorGUI.LabelField(new Rect(new Vector2(WindowRect.width - 85f, output.yPos - 9), new Vector2(70, 18)), output.label, gs);
            }
        }
        
        foreach (InterfaceType input in inputs)
        {
            Vector3 position = new Vector3(7f, input.yPos, 0f);
            DrawInterfaceAt(position, input.connectedNode != null);
            if (input.label != string.Empty)
            {
                EditorGUIUtility.labelWidth = 70f;
                var gs = GUI.skin.GetStyle("Label");
                gs.alignment = TextAnchor.UpperRight;
                EditorGUI.LabelField(new Rect(new Vector2(15f, input.yPos - 9), new Vector2(70, 18)), input.label, gs);
            }
        }
    } 

    private void DrawInterfaceAt(Vector3 position, bool connected)
    {
        DrawCircle(position, 6f, Color.blue);
        DrawCircle(position, 4f, Color.white);

        if (connected)
        {
            DrawCircle(position, 3f, Color.blue);
        }
    }

    private void DrawCircle(Vector3 position, float radius, Color color)
    {
        Handles.color = color;
        Handles.DrawSolidDisc(position, Vector3.back, radius);
    }

    public int OutputClicked(Vector2 mousePos)
    {
        int chosenOutput = -1;
        for (int i = 0; i < outputs.Count; i++)
        {
            Vector2 delta = new Vector2 (WindowRect.x + WindowRect.width - 7f, WindowRect.y + outputs[i].yPos) - mousePos;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
            if (dist < 6)
            {
                selectedOutputPos = new Vector2 (WindowRect.width - 7f, outputs[i].yPos);
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
            Vector2 delta = new Vector2(WindowRect.x + 7f, WindowRect.y + inputs[i].yPos) - mousePos;
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
            Vector2 delta = new Vector2(WindowRect.x + 7f, WindowRect.y + inputs[i].yPos) - mousePos;
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
        foreach (InterfaceType output in outputs)
        {
            if (output.connectedNode != null)
                DrawCurvesFromOutput(output);
        }
    }

    private void DrawCurvesFromOutput(InterfaceType output)
    {
        Rect start = new Rect()
        {
            x = WindowRect.x + WindowRect.width - 7f,
            y = WindowRect.y + output.yPos,
            width = 1,
            height = 1
        };
        
        Vector2 inputPos = output.connectedNode.GetInputPos(output.interfaceIndex);
        Rect end = new Rect()
        {
            x = inputPos.x,
            y = inputPos.y,
            width = 1,
            height = 1
        };
        BossEditor.DrawCurve(start, end);
    }

    private Vector2 GetInputPos(int inputIndex)
    {
        Vector2 inputPos = new Vector2(7f, inputs[inputIndex].yPos);
        return inputPos + new Vector2(WindowRect.x, WindowRect.y);
    }

    public Vector2 GetSelectedOutputPos()
    {
        return selectedOutputPos + new Vector2(WindowRect.x, WindowRect.y);
    }

    public void SetInput(int inputIndex, BaseNode outNode, int outputIndex)
    {
        if (outNode == this) return;    // Can't connect to self
        
        if (outNode.OutputIsConnected(outputIndex))
        {
            var originalInterface = outNode.GetConnectedInterfaceFromOutput(outputIndex);
            originalInterface.connectedNode.DisconnectInput(originalInterface.interfaceIndex);
        }

        var input = inputs[inputIndex];
        if (input.connectedNode != null)
            input.connectedNode.DisconnectOutput(input.interfaceIndex);

        input.connectedNode = outNode;
        input.interfaceIndex = outputIndex;
        inputs[inputIndex] = input;
        
        outNode.SetOutput(outputIndex, this, inputIndex);
    }

    private void SetOutput(int outputIndex, BaseNode node, int inputIndex)
    {
        var output = outputs[outputIndex];
        output.connectedNode = node;
        output.interfaceIndex = inputIndex;
        outputs[outputIndex] = output;
    }

    public void DeleteNode()
    {
        foreach (var input in inputs)
        {
            if (input.connectedNode != null)
                input.connectedNode.DisconnectOutput(input.interfaceIndex);
        }
        foreach (var output in outputs)
        {
            if (output.connectedNode != null)
                output.connectedNode.DisconnectInput(output.interfaceIndex);
        }
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

    private bool OutputIsConnected(int outputIndex)
    {
        return outputs[outputIndex].connectedNode != null;
    }

    public bool InputIsConnected(int inputIndex)
    {
        return inputs[inputIndex].connectedNode != null;
    }

    public InterfaceType GetConnectedInterfaceFromInput(int inputIndex)
    {
        return inputs[inputIndex];
    }

    private InterfaceType GetConnectedInterfaceFromOutput(int outputIndex)
    {
        return outputs[outputIndex];
    }

    protected void SetInput(float yPos, int id = 0, string label = "")
    {
        for (int i = 0; i < inputs.Count; i++)
        {
            if (inputs[i].identifier == id)
            {
                var input = inputs[i];
                input.yPos = yPos;
                input.label = label;
                inputs[i] = input;
                break; 
            }
        }
    }

    protected void SetOutput(float yPos, int id = 0, string label = "")
    {
        for (int i = 0; i < outputs.Count; i++)
        {
            if (outputs[i].identifier == id)
            {
                var output = outputs[i];
                output.yPos = yPos;
                output.label = label;
                outputs[i] = output;
                break;
            }
        }
    }

    public abstract void Activate();

    public void CallNext(int id = 0)
    {
        foreach (var output in outputs)
        {
            if (output.identifier == id && output.connectedNode != null)
                output.connectedNode.Activate();
        }
    }

}
