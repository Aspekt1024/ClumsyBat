using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {
    
    public enum InterfaceTypes
    {
        Event, Object
    }

    public Rect WindowRect;
    public string WindowTitle = "Untitled";
    public bool IsSelected;
    
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

    private BaseEditor editor;
    private Vector2 selectedOutputPos;

    private bool IsDragged;
    private Rect OffsetRect;
    private Vector2 dragOffset;

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
    
    public void DrawNodeWindow(BaseEditor editorRef, int id, Vector2 canvasOffset)
    {
        editor = editorRef; // TODO idk... this doesn't seem right to be here

        WindowRect.position = SnapToGrid(WindowRect.position);
        OffsetRect = new Rect(SnapToGrid(WindowRect.position + dragOffset) + canvasOffset, WindowRect.size);
        
        if (IsSelected)
        {
            GUI.skin = (GUISkin)EditorGUIUtility.Load("NodeWindowSkin.guiskin");
        }
        else
        {
            GUI.skin = (GUISkin)EditorGUIUtility.Load("NodeNormalSkin.guiskin");
        }
        
        GUI.Box(OffsetRect, WindowTitle);
        GUI.BeginGroup(OffsetRect);
        Draw();
        GUI.EndGroup();
        
    }

    private Vector2 SnapToGrid(Vector2 pos)
    {
        Vector2 snappedPos = new Vector2
        {
            x = NodeGUIElements.GridSpacing * Mathf.Round(pos.x / NodeGUIElements.GridSpacing),
            y = NodeGUIElements.GridSpacing * Mathf.Round(pos.y / NodeGUIElements.GridSpacing)
        };
        return snappedPos;
    }

    private void Draw()
    {
        DrawWindow();
    }

    public virtual void DrawWindow()
    {
        DrawInterfaces();
    }

    public void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (OffsetRect.Contains(e.mousePosition))
                    {
                        IsDragged = true;

                        if (e.shift)
                            IsSelected = !IsSelected;
                        else
                        {
                            if (!IsSelected)
                                editor.DeselectAllNodes();

                            IsSelected = true;
                        }

                        GUI.changed = true;
                    }
                }
                break;
            case EventType.mouseUp:
                if (IsDragged)
                {
                    WindowRect.position += dragOffset;
                    dragOffset = Vector2.zero;
                    IsDragged = false;
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && IsDragged)
                {
                    dragOffset += e.delta;
                    e.Use();
                    return;
                }
                break;
        }
    }
    
    public virtual void SetWindowRect(Vector2 position)
    {
        WindowRect = new Rect(position.x, position.y, WindowRect.width, WindowRect.height);
    }

    public bool Contains(Vector2 pos)
    {
        return new Rect(OffsetRect.position, WindowRect.size).Contains(pos);
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
            Vector2 delta = new Vector2 (OffsetRect.x + WindowRect.width - 7f, OffsetRect.y + outputs[i].yPos) - mousePos;
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

    public void SetSelectedOutputPosFromIndex(int outputIndex)
    {
        selectedOutputPos = new Vector2(WindowRect.width - 7f, outputs[outputIndex].yPos);
    }

    public int InputClicked(Vector2 mousePos)
    {
        int chosenInput = -1;
        for (int i = 0; i < inputs.Count; i++)
        {
            Vector2 delta = new Vector2(OffsetRect.x + 7f, OffsetRect.y + inputs[i].yPos) - mousePos;
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
            Vector2 delta = new Vector2(OffsetRect.x + 7f, OffsetRect.y + inputs[i].yPos) - mousePos;
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
            x = OffsetRect.x + WindowRect.width - 7f,
            y = OffsetRect.y + output.yPos,
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
        Vector2 inputPos = new Vector2(7f, inputs[inputIndex].yPos) + OffsetRect.position;
        return inputPos;
    }

    public Vector2 GetSelectedOutputPos()
    {
        return selectedOutputPos + OffsetRect.position;
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
