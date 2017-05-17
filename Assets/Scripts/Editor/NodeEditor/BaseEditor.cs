using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class BaseEditor : EditorWindow {
    
    public NodeData NodeData;
    public Vector2 NodeDrag;
    public Vector2 CanvasOffset;
    public Vector2 CanvasDrag;
    public NodeInterface DraggedInterface;

    protected BossDataContainer BaseContainer;
    protected string EditorLabel;

    protected ColourThemes colourTheme;

    protected enum ColourThemes
    {
        Blue, Green, Black
    }

    protected BaseNode _currentNode;

    private Texture2D _bg;
    private BaseEditorMouseInput _mouseInput;
    private Vector2 _mousePos;
    private Vector2 startMousePos;
    private float timeSinceUpdate;
    
    protected abstract void SetEditorTheme();
    
    public virtual void LoadEditor(BossDataContainer obj)
    {
        SetEditorTheme();
        BaseContainer = obj;

        SetRootContainerToSelf();

        LoadNodeData();

        foreach (var node in NodeData.Nodes)
        {
            node.ParentEditor = this;
        }
    }

    public void Drag(Vector2 delta)
    {
        CanvasDrag += delta;
        Repaint();
    }
    
    private void SetRootContainerToSelf()
    {
        if (BaseContainer.RootContainer == null)
            BaseContainer.RootContainer = BaseContainer;
    }

    protected abstract void LoadNodeData();

    protected void CreateNewNodeData(string nodeDataPath)
    {
        NodeData = new NodeData();
        // TODO save this somewherE?
        //AssetDatabase.CreateAsset(NodeData, nodeDataPath);

    }

    protected virtual void OnEnable()
    {
        if (_mouseInput == null) _mouseInput = new BaseEditorMouseInput(this);
        
        if (BaseContainer != null)
        {
            LoadEditor(BaseContainer);
        }
    }

    private void OnLostFocus()
    {
        if (NodeData == null || BaseContainer == null) return;
        SaveActionData();
    }

    public void DeselectAllNodes()
    {
        foreach (BaseNode node in NodeData.Nodes)
        {
            node.Transform.IsSelected = false;
        }
    }

    public void MoveAllSelectedNodes(Vector2 delta)
    {
        foreach (BaseNode node in NodeData.Nodes)
        {
            if (node.Transform.IsSelected)
                node.WindowRect.position += delta;
        }
    }
    
    public void SaveActionData()
    {
        Debug.Log("TODO save action");
        // TODO this
        //BossEditorSave editorSaveScript = new BossEditorSave();
        //editorSaveScript.CreateActionFile(BaseContainer, NodeData);
    }
    
    private void OnGUI()
    {
        if (BaseContainer == null) return;
        
        DrawBackground();
        DrawHeading();
        DrawNodeWindows();

        ProcessEvents();
    }

    private void ProcessEvents()
    {
        Event e = Event.current;
        _mousePos = e.mousePosition;
        ProcessNodeEvents();
        _mouseInput.ProcessMouseEvents(e);

        if (GUI.changed) Repaint();
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
        }
        return bgColour;
    }

    private void DrawHeading()
    {
        var cStyle = new GUIStyle();
        cStyle.normal.textColor = Color.white;
        cStyle.fontSize = 20;
        cStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField(EditorLabel, cStyle);
    }

    public BaseNode GetSelectedNode()
    {
        int index = -1;
        for (int i = NodeData.Nodes.Count - 1; i >= 0; i--)
        {
            if (NodeData.Nodes[i].Contains(_mousePos))
            {
                index = i;
                _currentNode = NodeData.Nodes[i];
                break;
            }
        }
        return index >= 0 ? NodeData.Nodes[index] : null;
    }

    public void SetSelectedNode(BaseNode node)
    {
        _currentNode = node;
    }

    private void DrawNodeWindows()
    {
        for (int i = 0; i < NodeData.Nodes.Count; i++)
        {
            NodeData.Nodes[i].DrawNodeWindow(CanvasOffset + CanvasDrag);
        }
    }

    private void ProcessNodeEvents()
    {
        for (int i = 0; i < NodeData.Nodes.Count; i++)
        {
            NodeData.Nodes[i].ProcessEvents(Event.current);
        }
    }

    public void StartMovingEditorCanvas()
    {
        CanvasDrag = Vector2.zero;
        startMousePos = _mousePos;
    }

    public void StopMovingEditorCanvas()
    {
        CanvasOffset += CanvasDrag;
        CanvasDrag = Vector2.zero;
        Repaint();
    }

    public virtual void AddNode(BaseNode newNode)
    {
        newNode.InitialiseNode(_mousePos - CanvasOffset, this);
        newNode.SetupNode(BaseContainer);
        NodeData.Nodes.Add(newNode);
    }

    public void RemoveNode(BaseNode node)
    {
        node.DeleteNode();
        NodeData.Nodes.Remove(node);

        // TODO remove from save file
        //if (AssetDatabase.Contains(node))
        //    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node));
    }

    private float SnapToGrid(float pos)
    {
        const float windowPosIncrement = 10f;
        return windowPosIncrement * Mathf.Round(pos / windowPosIncrement);
    }
    
    public void MoveToStart()
    {
        foreach (var node in NodeData.Nodes)
        {
            if (node.GetType().Equals(typeof(StartNode)))
            {
                CanvasOffset = _mousePos - new Vector2(node.Transform.WindowRect.x, node.Transform.WindowRect.y);
                break;
            }
        }
    }

    public bool StartExists()
    {
        bool startFound = false;
        foreach (var node in NodeData.Nodes)
        {
            if (node.GetType().Equals(typeof(StartNode)))
            {
                startFound = true;
                break;
            }
        }
        return startFound;
    }
}
