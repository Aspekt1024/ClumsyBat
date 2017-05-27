using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class BaseEditor : EditorWindow {
    
    public List<BaseNode> Nodes;
    public Vector2 NodeDrag;
    public Vector2 CanvasOffset;
    public Vector2 CanvasDrag;
    public NodeInterface DraggedInterface;
    public BehaviourSet BehaviourSet;
    public string EditorLabel;
    public int ActiveStateNodeID;

    protected ColourThemes colourTheme;
    
    protected enum ColourThemes
    {
        Blue, Green, Black
    }
    
    private Texture2D _bg;
    private BaseEditorMouseInput _mouseInput;
    private Vector2 _mousePos;
    private NodeRuntimeBorders runtimeBorders;

    protected NodeEditorMenu nodeMenu;
    protected BaseNode _currentNode;
    
    protected abstract void SetEditorTheme();
    
    public virtual void LoadEditor(BehaviourSet behaviourSet)
    {
        if (nodeMenu == null) nodeMenu = new NodeEditorMenu(this);
        
        nodeMenu.SaveCurrentMenuState();
        
        BehaviourSet = behaviourSet;
        SetParentMachine();
        SetEditorTheme();
        
        try
        {
            NodeEditorSaveHandler.Load(this);
        }
        catch
        {
            Nodes = new List<BaseNode>();
        }

        nodeMenu.UpdateSystemModel();
    }

    public void Drag(Vector2 delta)
    {
        CanvasDrag += delta;
        if (CanvasDrag.magnitude > 10)
            _mouseInput.CanvasWasDragged = true;

        Repaint();
    }
    
    private void SetParentMachine()
    {
        if (BehaviourSet.ParentMachine == null && BehaviourSet.IsType<StateMachine>())
            BehaviourSet.ParentMachine = (StateMachine)BehaviourSet;
    }
    
    protected virtual void OnEnable()
    {
        if (_mouseInput == null) _mouseInput = new BaseEditorMouseInput(this);
        
        if (BehaviourSet != null)
        {
            LoadEditor(BehaviourSet);
        }
    }

    private void OnLostFocus()
    {
        if (Nodes == null || Nodes.Count == 0) return;
        Save();
    }

    private void Save()
    {
        NodeEditorSaveHandler.Save(this);
    }

    public void DeselectAllNodes()
    {
        foreach (BaseNode node in Nodes)
        {
            node.Transform.IsSelected = false;
        }
        Repaint();
    }

    public void MoveAllSelectedNodes(Vector2 delta)
    {
        foreach (BaseNode node in Nodes)
        {
            if (node.Transform.IsSelected)
                node.WindowRect.position += delta;
        }
    }
    
    private void OnGUI()
    {
        if (BehaviourSet == null) return;
        
        // Last to be called is drawn on top
        DrawBackground();
        DrawNodeWindows();
        ShowRuntimeBorders();
        nodeMenu.Draw();

        ProcessEvents();
    }

    private void ShowRuntimeBorders()
    {
        if (!Application.isPlaying) return;

        if (runtimeBorders == null)
            runtimeBorders = new NodeRuntimeBorders(this);

        runtimeBorders.Update();
    }

    private void ProcessEvents()
    {
        Event e = Event.current;
        _mousePos = e.mousePosition;
        ProcessNodeEvents();
        _mouseInput.ProcessMouseEvents(e);

        if (GUI.changed)
            Repaint();
    }

    private void DrawBackground()
    {
        if (_bg == null)
        {
            _bg = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _bg.SetPixel(0, 0, GetBgColour());
            _bg.Apply();
        }
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), _bg, ScaleMode.StretchToFill);
        DrawGrid(250, new Color(1f, 1f, 1f, 0.4f));
        DrawGrid(50, new Color(1f, 1f, 1f, 0.1f));
        DrawGrid(10, new Color(1f, 1f, 1f, 0.03f));
    }

    private void DrawGrid(float gridSpacing, Color gridColour)
    {
        Handles.BeginGUI(); 
        Handles.color = gridColour;

        float hPos = (CanvasOffset.x + CanvasDrag.x) % gridSpacing;
        float vPos = (CanvasOffset.y + CanvasDrag.y) % gridSpacing;

        while (hPos < position.width)
        {
            Handles.DrawLine(new Vector3(hPos, 0, 0), new Vector3(hPos, position.height, 0));
            hPos += gridSpacing;
        }

        while (vPos < position.height)
        {
            Handles.DrawLine(new Vector3(0, vPos, 0), new Vector3(position.width, vPos, 0));
            vPos += gridSpacing;
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private Color GetBgColour()
    {
        Color bgColour = new Color();
        switch (colourTheme)
        {
            case ColourThemes.Blue:
                bgColour = new Color(0.1f, 0.1f, 0.2f);
                break;
            case ColourThemes.Green:
                bgColour = new Color(0.1f, 0.2f, 0.1f);
                break;
            case ColourThemes.Black:
                bgColour = new Color(0.17f, 0.17f, 0.17f);
                break;
        }
        return bgColour;
    }

    public BaseNode GetSelectedNode()
    {
        int index = -1;
        for (int i = Nodes.Count - 1; i >= 0; i--)
        {
            if (Nodes[i].Contains(_mousePos))
            {
                index = i;
                _currentNode = Nodes[i];
                break;
            }
        }
        return index >= 0 ? Nodes[index] : null;
    }

    public void SetSelectedNode(BaseNode node)
    {
        _currentNode = node;
    }

    private void DrawNodeWindows()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].DrawConnections();
        }
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].DrawNodeWindow(CanvasOffset + CanvasDrag);
        }
    }

    private void ProcessNodeEvents()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].ProcessEvents(Event.current);
        }
    }
    
    public void StopMovingEditorCanvas()
    {
        CanvasOffset += CanvasDrag;
        CanvasDrag = Vector2.zero;
    }

    public virtual void AddNode(BaseNode newNode)
    {
        newNode.InitialiseNode(_mousePos - CanvasOffset, this);
        newNode.SetupNode(BehaviourSet);
        newNode.ID = Nodes.Count;
        Nodes.Add(newNode);
    }

    public void RemoveNode(BaseNode node)
    {
        node.DeleteNode();
        Nodes.Remove(node);

        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].ID = i;
        }
    }

    private float SnapToGrid(float pos)
    {
        const float windowPosIncrement = 10f;
        return windowPosIncrement * Mathf.Round(pos / windowPosIncrement);
    }
    
    public void MoveToStart()
    {
        foreach (var node in Nodes)
        {
            if (node.GetType().Equals(typeof(StartNode)))
            {
                CanvasOffset = _mousePos - new Vector2(node.WindowRect.x, node.WindowRect.y);
                break;
            }
        }
    }

    public bool StartExists()
    {
        bool startFound = false;
        foreach (var node in Nodes)
        {
            if (node.GetType().Equals(typeof(StartNode)))
            {
                startFound = true;
                break;
            }
        }
        return startFound;
    }

    public StartNode GetStartNode()
    {
        foreach (var node in Nodes)
        {
            if (node.IsType<StartNode>())
                return (StartNode)node;
        }
        return null;
    }
}
