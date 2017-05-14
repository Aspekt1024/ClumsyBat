using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {

    public bool IsSelected;

    public enum InterfaceTypes
    {
        Event, Object
    }

    public Rect WindowRect;
    public Vector2 BasePosition;
    public string WindowTitle = "Untitled";
    
    public List<InterfaceType> inputs = new List<InterfaceType>();
    public List<InterfaceType> outputs = new List<InterfaceType>();
    
    [HideInInspector]
    public BossDataContainer DataContainer;
    
    public BaseAction Action;

    [System.Serializable]
    public struct InterfaceType
    {
        public float yPos;
        public BaseNode connectedNode;
        public int connectedInterfaceIndex;
        public int identifier;
        public string label;
        public InterfaceTypes type; 
    }

    private Vector2 selectedOutputPos;
    private Vector2 offset;
    
    protected abstract void AddInterfaces();

    public virtual void SetupNode(BossDataContainer dataContainer)
    {
        DataContainer = dataContainer;
        AddInterfaces();
        SaveThisNodeAsset();
    }

    protected void SaveThisNodeAsset()
    {
        string subFolder = "";
        if (DataContainer.IsType<BossState>())
        {
            subFolder = DataContainer.name + "Data";
        }

        EditorHelpers.SaveNodeEditorAsset(this, DataContainer.RootContainer, subFolder, GetType().ToString());
    }

    public void DrawNodeWindow(BaseEditor editor, int id, Vector2 canvasOffset)
    {
        offset = canvasOffset;

        WindowRect.x = NodeGUIElements.GridSpacing * Mathf.Round(WindowRect.x / NodeGUIElements.GridSpacing);
        WindowRect.y = NodeGUIElements.GridSpacing * Mathf.Round(WindowRect.y / NodeGUIElements.GridSpacing);
        
        Rect nodeRect = WindowRect;
        nodeRect.x += offset.x;
        nodeRect.y += offset.y;

        GUISkin mySkin = null;

        GUI.color = Color.white;
        if (IsSelected)
        {
            mySkin = (GUISkin)EditorGUIUtility.Load("NodeWindowSkin.guiskin");
        }
        else
        {
            mySkin = (GUISkin)EditorGUIUtility.Load("NodeNormalSkin.guiskin");
        }

        GUIStyle style = mySkin.GetStyle("Window");

        nodeRect = GUI.Window(id, nodeRect, DrawWindowCallback, WindowTitle, style);
        
        if (IsSelected && editor.GetNumSelectedNodes() > 1)
        {
            // TODO tidy this up.. nodes don't move together because drag.magnitude is often == 0
            // making for jittery behaviour
            nodeRect = GUI.Window(id, nodeRect, DrawWindowCallback, WindowTitle, style);
            Vector2 drag = WindowRect.position - nodeRect.position + offset;
            editor.NodeDrag += drag;
        }
        else
        {
            WindowRect.position = nodeRect.position - offset;
        }
    }

    private void DrawWindowCallback(int id)
    {
        DrawWindow();
        GUI.DragWindow();
    }

    public virtual void DrawWindow()
    {
        DrawInterfaces();
    }
    
    public virtual void SetWindowRect(Vector2 position)
    {
        WindowRect = new Rect(position.x, position.y, WindowRect.width, WindowRect.height);
    }

    public void DragIfSelected(Vector2 delta)
    {
        if (!IsSelected) return;
        WindowRect.position -= delta;
    }

    public bool Contains(Vector2 pos)
    {
        return new Rect(WindowRect.position + offset, WindowRect.size).Contains(pos);
    }

    protected void AddOutput(int id = 0, InterfaceTypes ifaceType = InterfaceTypes.Event)
    {
        InterfaceType outputPos = new InterfaceType()
        {
            yPos = 0,
            connectedNode = null,
            connectedInterfaceIndex = 0,
            identifier = id,
            label = "",
            type = ifaceType
        };
        outputs.Add(outputPos);
    }

    protected void AddInput(int id = 0, InterfaceTypes ifaceType = InterfaceTypes.Event)
    {
        InterfaceType inputPos = new InterfaceType()
        {
            yPos = 0,
            connectedNode = null,
            connectedInterfaceIndex = 0,
            identifier = id,
            label = "",
            type = ifaceType
        };
        inputs.Add(inputPos);
    }

    protected void DrawInterfaces()
    {
        foreach (InterfaceType output in outputs)
        {
            DrawOutputInterface(output);
        }
        
        foreach (InterfaceType input in inputs)
        {
            DrawInputInterface(input);
        }
    } 

    protected void DrawOutputInterface(InterfaceType output)
    {
        Vector3 position = new Vector3(WindowRect.width - 7f, output.yPos, 0f);
        DrawInterfaceAt(position, output.connectedNode != null, output.type);
        if (output.label != string.Empty)
        {
            EditorGUIUtility.labelWidth = 70f;
            var gs = GUI.skin.GetStyle("Label");
            gs.alignment = TextAnchor.UpperRight;
            gs.normal.textColor = Color.black;
            EditorGUI.LabelField(new Rect(new Vector2(WindowRect.width - 85f, output.yPos - 9), new Vector2(70, 18)), output.label, gs);
        }
    }

    protected void DrawInputInterface(InterfaceType input)
    {
        Vector3 position = new Vector3(7f, input.yPos, 0f);
        DrawInterfaceAt(position, input.connectedNode != null, input.type);
        if (input.label != string.Empty)
        {
            EditorGUIUtility.labelWidth = 70f;
            var gs = GUI.skin.GetStyle("Label");
            gs.alignment = TextAnchor.UpperLeft;
            gs.normal.textColor = Color.black;
            EditorGUI.LabelField(new Rect(new Vector2(15f, input.yPos - 9), new Vector2(70, 18)), input.label, gs);
        }
    }

    private void DrawInterfaceAt(Vector3 position, bool connected, InterfaceTypes type)
    {
        Color ringCol = Color.blue;
        Color connCol = new Color(0.5f, 0.5f, 1f);
        if (type == InterfaceTypes.Object) {
            ringCol = Color.red;
            connCol = new Color(1f, 0.5f, 0.5f);
        }

        DrawCircle(position, 6f, ringCol);
        DrawCircle(position, 4f, Color.white);

        if (connected)
        {
            DrawCircle(position, 3f, connCol);
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
            Vector2 delta = new Vector2 (WindowRect.x + offset.x + WindowRect.width - 7f, WindowRect.y + offset.y + outputs[i].yPos) - mousePos;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
            if (dist < 6)
            {
                selectedOutputPos = new Vector2 (WindowRect.width - 7f + offset.x, outputs[i].yPos + offset.y);
                chosenOutput = i;
                break;
            }
        }
        return chosenOutput;
    }

    public void SetSelectedOutputPosFromIndex(int outputIndex)
    {
        selectedOutputPos = new Vector2(WindowRect.width - 7f, outputs[outputIndex].yPos);
    }

    public int InputClicked(Vector2 mousePos)
    {
        int chosenInput = -1;
        for (int i = 0; i < inputs.Count; i++)
        {
            Vector2 delta = new Vector2(WindowRect.x + 7f + offset.x, WindowRect.y + inputs[i].yPos + offset.y) - mousePos;
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
            Vector2 delta = new Vector2(WindowRect.x + offset.x + 7f, WindowRect.y + offset.y + inputs[i].yPos) - mousePos;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
            if (dist < 10)
            {
                chosenInput = i;
                break;
            }
        }
        return chosenInput;
    }
    
    public void DrawConnections()
    {
        foreach (InterfaceType output in outputs)
        {
            if (output.connectedNode != null)
                DrawConnectionFromOutput(output);
        }
    }

    private void DrawConnectionFromOutput(InterfaceType output)
    {
        Rect start = new Rect()
        {
            x = WindowRect.x + offset.x + WindowRect.width - 7f,
            y = WindowRect.y + offset.y + output.yPos,
            width = 1,
            height = 1
        };
        
        Vector2 inputPos = output.connectedNode.GetInputPos(output.connectedInterfaceIndex);
        Rect end = new Rect()
        {
            x = inputPos.x,
            y = inputPos.y,
            width = 1,
            height = 1
        };
        BaseEditor.DrawConnection(start, end, output.type);
    }

    private Vector2 GetInputPos(int inputIndex)
    {
        Vector2 inputPos = new Vector2(7f, inputs[inputIndex].yPos) + WindowRect.position + offset;
        return inputPos;
    }

    public Vector2 GetSelectedOutputPos()
    {
        return selectedOutputPos + WindowRect.position + offset;
    }

    public void ConnectInput(int inputIndex, BaseNode outNode, int outputIndex)
    {
        if (outNode == this) return;    // Can't connect to self
        
        if (outNode.OutputIsConnected(outputIndex))
        {
            var originalInterface = outNode.GetConnectedInterfaceFromOutput(outputIndex);
            originalInterface.connectedNode.DisconnectInput(originalInterface.connectedInterfaceIndex);
        }

        var input = inputs[inputIndex];
        if (input.connectedNode != null)
            input.connectedNode.DisconnectOutput(input.connectedInterfaceIndex);

        input.connectedNode = outNode;
        input.connectedInterfaceIndex = outputIndex;
        inputs[inputIndex] = input;
        
        outNode.ConnectOutput(outputIndex, this, inputIndex);
    }

    private void ConnectOutput(int outputIndex, BaseNode node, int inputIndex)
    {
        var output = outputs[outputIndex];
        output.connectedNode = node;
        output.connectedInterfaceIndex = inputIndex;
        outputs[outputIndex] = output;
    }

    public virtual void DeleteNode()
    {
        foreach (var input in inputs)
        {
            if (input.connectedNode != null)
                input.connectedNode.DisconnectOutput(input.connectedInterfaceIndex);
        }
        foreach (var output in outputs)
        {
            if (output.connectedNode != null)
                output.connectedNode.DisconnectInput(output.connectedInterfaceIndex);
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

    public bool IsType<T>() where T : BaseNode
    {
        return GetType().Equals(typeof(T));
    }

    public BaseAction GetAction()
    {
        if (Action == null)
            CreateAction();

        return Action;
    }
    
    protected abstract void CreateAction();


    #region Assign node interfaces to Action

    public virtual void ConvertInterfaces()
    {
        ConvertInputInterfaces();
        ConvertOutputInterfaces();
    }

    private void ConvertInputInterfaces()
    {
        foreach(var input in inputs)
        {
            if (input.connectedNode != null)
            {
                var actionInput = ConvertInterface(input);
                Action.inputs.Add(actionInput);
            }
        }
    }

    private void ConvertOutputInterfaces()
    {
        foreach (var output in outputs)
        {
            if (output.connectedNode != null)
            {
                var actionOutput = ConvertInterface(output);
                Action.outputs.Add(actionOutput);
            }
        }
    }
    
    private static BaseAction.InterfaceType ConvertInterface(InterfaceType iface)
    {
        return new BaseAction.InterfaceType()
        {
            identifier = iface.identifier,
            connectedAction = iface.connectedNode.Action,
            connectedInterfaceIndex = iface.connectedInterfaceIndex
        };
    }
    #endregion
}
