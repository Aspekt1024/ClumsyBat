using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class BaseEditor : EditorWindow {
    
    public BossEditorNodeData NodeData; // Node data cannot be saved in the editor persistently
    public bool ConnectionMode;
    public bool MoveEditorMode;
    public Vector2 NodeDrag;
    
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
    private Vector2 canvasDrag;
    private Vector2 canvasOffset = Vector2.zero;
    private float timeSinceUpdate;


    protected abstract void SetEditorTheme();

    public virtual void LoadEditor(BossDataContainer obj)
    {
        SetEditorTheme();
        BaseContainer = obj;

        SetRootContainerToSelf();

        LoadNodeData();
    }

    public void Drag(Vector2 delta)
    {
        if (!MoveEditorMode && !ConnectionMode)
        {
            Vector2 dragAmount = new Vector2(Mathf.Floor(NodeDrag.x / NodeGUIElements.GridSpacing), Mathf.Floor(NodeDrag.y / NodeGUIElements.GridSpacing)) * NodeGUIElements.GridSpacing;
            if (dragAmount.magnitude > 0)
            {
                NodeDrag -= dragAmount;

                foreach (BaseNode node in NodeData.Nodes)
                {
                    node.DragIfSelected(dragAmount / 2);
                }
                Repaint();
            }
        }
        else if (delta.magnitude > 0.01f)
        {
            Repaint();
        }
    }

    public void DeselectAllNodes()
    {
        foreach (BaseNode node in NodeData.Nodes)
        {
            node.IsSelected = false;
        }
    }

    private void SetRootContainerToSelf()
    {
        if (BaseContainer.RootContainer == null)
            BaseContainer.RootContainer = BaseContainer;
    }

    protected abstract void LoadNodeData();

    protected void CreateNewNodeData(string nodeDataPath)
    {
        NodeData = CreateInstance<BossEditorNodeData>();
        NodeData.Nodes = new List<BaseNode>();
        AssetDatabase.CreateAsset(NodeData, nodeDataPath);
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

    public void SaveActionData()
    {
        BossEditorSave editorSaveScript = new BossEditorSave();
        editorSaveScript.CreateActionFile(BaseContainer, NodeData);
    }
    
    private void Update()
    {
        if (!MoveEditorMode) return;

        canvasDrag = _mousePos - startMousePos;

        timeSinceUpdate += Time.deltaTime;
        if (timeSinceUpdate > 0.05f)
        {
            Repaint();
            timeSinceUpdate = 0f;
        }
    }

    private void OnGUI()
    {
        if (BaseContainer == null) return;

        Event e = Event.current;
        _mousePos = e.mousePosition;
        _mouseInput.ProcessMouseEvents(e);
        
        DrawBackground();
        DrawHeading();
        DrawNodeWindows();
        DrawConnections();
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

        float hPos = (canvasOffset.x + canvasDrag.x) % gridSpacing;
        float vPos = (canvasOffset.y + canvasDrag.y) % gridSpacing;

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
        BeginWindows();
        for (int i = 0; i < NodeData.Nodes.Count; i++)
        {
            NodeData.Nodes[i].DrawNodeWindow(this, i, canvasOffset + canvasDrag);
        }
        EndWindows();
    }

    public void StartMovingEditorCanvas()
    {
        startMousePos = _mousePos;
        MoveEditorMode = true;
    }

    public void StopMovingEditorCanvas()
    {
        MoveEditorMode = false;
        canvasOffset += canvasDrag;
        canvasDrag = Vector2.zero;
        Repaint();
    }

    private void DrawConnections()
    {
        if (ConnectionMode && _currentNode != null)
        {
            Rect mouseRect = new Rect(_mousePos.x, _mousePos.y, 10, 10);
            Rect outputRect = new Rect(_currentNode.GetSelectedOutputPos().x - canvasOffset.x, _currentNode.GetSelectedOutputPos().y - canvasOffset.y, 1, 1);
            DrawConnection(outputRect, mouseRect);
            Repaint();
        }

        foreach (var node in NodeData.Nodes)
        {
            node.DrawConnections();
        }
    }

    public static void DrawConnection(Rect start, Rect end, BaseNode.InterfaceTypes outputType = BaseNode.InterfaceTypes.Event)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0f);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0f);
        Color shadowCol = new Color(0.7f, 0.7f, 1f);
        if (outputType == BaseNode.InterfaceTypes.Object)
            shadowCol = new Color(1f, 0.7f, 0.7f);

        Vector3 tanScale = GetTanScale(startPos, endPos);
        Vector3 startTan = startPos + tanScale;
        Vector3 endTan = endPos - tanScale;

        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol * new Color(1f, 1f, 1f, 0.06f), null, (i + 1) * 7);
        }
        Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, 3);
    }

    private static Vector3 GetTanScale(Vector3 startPos, Vector3 endPos)
    {
        Vector3 tanScale = new Vector3(50f, 0f, 0f);

        float dX = startPos.x - endPos.x;
        if (dX > 0)
        {
            float dY = startPos.y - endPos.y;
            if (dY < 0) dY = -dY;
            tanScale += new Vector3(dX, dY, 0f) / 2f;
        }
        return tanScale;
    }

    public virtual void AddNode(BaseNode newNode)
    {
        newNode.SetWindowRect(_mousePos);
        newNode.SetupNode(BaseContainer);
        NodeData.Nodes.Add(newNode);
    }

    public void RemoveNode(BaseNode node)
    {
        node.DeleteNode();
        NodeData.Nodes.Remove(node);

        if (AssetDatabase.Contains(node))
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node));
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
                canvasOffset = _mousePos - new Vector2(node.WindowRect.x, node.WindowRect.y);
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

    public int GetNumSelectedNodes()
    {
        int numSelectedNodes = 0;
        foreach (BaseNode node in NodeData.Nodes)
        {
            if (node.IsSelected)
                numSelectedNodes++;
        }
        return numSelectedNodes;
    }
}
