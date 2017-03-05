using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class BaseEditor : EditorWindow {

    public BossEditorNodeData NodeData;
    public List<BaseNode> Nodes;
    public bool ConnectionMode;
    public bool MoveEditorMode;

    protected ScriptableObject ParentObject;    // TODO create Base class for BossCreator and BossState (e.g. BaseBossEditable)
    protected string EditorLabel;

    protected ColourThemes colourTheme;

    protected enum ColourThemes
    {
        Blue, Green
    }

    protected BaseNode _currentNode;
    
    private Texture2D _bg;
    private BossEditorMouseInput _mouseInput;
    private Vector2 _mousePos;
    private Vector2 startMousePos;
    private Vector2 canvasDisplacement;
    private float timeSinceUpdate;
    
    protected abstract void SetEditorTheme();

    public virtual void LoadEditor(ScriptableObject obj)
    {
        SetEditorTheme();
        ParentObject = obj;
        
        if (NodeData == null)
        {
            NodeData = CreateInstance<BossEditorNodeData>();
            Nodes = new List<BaseNode>();
            SaveNodeDataAsset();
        }
        else
            Nodes = NodeData.Nodes;
    }

    private void SaveNodeDataAsset()
    {
        string dataFolder = EditorHelpers.GetAssetDataFolder(ParentObject);
        EditorHelpers.SaveObjectToFolder(NodeData, dataFolder, "NodeData");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    protected virtual void OnEnable()
    {
        if (_mouseInput == null) _mouseInput = new BossEditorMouseInput(this);

        if (ParentObject != null)
        {
            LoadEditor(ParentObject);
        }
    }

    protected virtual void OnLostFocus()
    {
        if (ParentObject != null)
        {
            SaveNodes();
        }
    }

    private void SaveNodes()
    {
        string dataFolder = EditorHelpers.GetAssetDataFolder(ParentObject);
        
        for (int i = 0; i < Nodes.Count; i++)
        {
            EditorHelpers.SaveObjectToFolder(Nodes[i], dataFolder, Nodes[i].WindowTitle);
            EditorUtility.SetDirty(Nodes[i]);
        }

        NodeData.Nodes = Nodes;
        EditorUtility.SetDirty(NodeData);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    
    private void Update()
    {
        if (!MoveEditorMode) return;
        canvasDisplacement = _mousePos - startMousePos;
        timeSinceUpdate += Time.deltaTime;
        if (timeSinceUpdate > 0.1f)
        {
            timeSinceUpdate -= 0.1f;
            Repaint();
        }
    }

    private void OnGUI()
    {
        // TODO if no object, show text to tell user to display one, else show title in editor of what the boss is
        if (ParentObject == null) return;

        Event e = Event.current;
        _mousePos = e.mousePosition;
        _mouseInput.ProcessMouseEvents(e);

        DrawBackground();
        DrawHeading();
        DrawNodeWindows();
        DrawNodeCurves();
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
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].WindowRect.Contains(_mousePos))
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
        BeginWindows();
        for (int i = 0; i < Nodes.Count; i++)
        {
            Rect nodeRect = Nodes[i].WindowRect;
            if (MoveEditorMode)
            {
                nodeRect.x = Nodes[i].OriginalRect.x + canvasDisplacement.x;
                nodeRect.y = Nodes[i].OriginalRect.y + canvasDisplacement.y;
            }
            Nodes[i].WindowRect = GUI.Window(i, nodeRect, DrawNodeWindow, Nodes[i].WindowTitle);
        }
        EndWindows();
    }

    public void StartMovingEditorCanvas()
    {
        startMousePos = _mousePos;
        MoveEditorMode = true;
        foreach (var node in Nodes)
        {
            node.OriginalRect = node.WindowRect;
        }
    }

    public void StopMovingEditorCanvas()
    {
        MoveEditorMode = false;
        foreach (var node in Nodes)
        {
            node.WindowRect = node.OriginalRect;
        }
        AddDisplacementToNodes();
    }

    private void DrawNodeWindow(int id)
    {
        Nodes[id].DrawWindow();
        if (!ConnectionMode)
        {
            GUI.DragWindow();
        }
    }

    private void DrawNodeCurves()
    {
        if (ConnectionMode && _currentNode != null)
        {
            Rect mouseRect = new Rect(_mousePos.x, _mousePos.y, 10, 10);
            Rect outputRect = new Rect(_currentNode.GetSelectedOutputPos().x, _currentNode.GetSelectedOutputPos().y, 1, 1);
            DrawCurve(outputRect, mouseRect);
            Repaint();
        }

        foreach (var node in Nodes)
        {
            node.DrawCurves();
        }
    }

    public static void DrawCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0f);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0f);
        Color shadowCol = new Color(0.7f, 0.7f, 1f, 0.06f);

        Vector3 tanScale = GetTanScale(startPos, endPos);
        Vector3 startTan = startPos + tanScale;
        Vector3 endTan = endPos - tanScale;

        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 7);
        }
        Handles.DrawBezier(startPos, endPos, startTan, endTan, new Color(0.7f, 0.7f, 1f), null, 3);
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
        newNode.SetupNode();
        Nodes.Add(newNode);
    }

    public void RemoveNode(BaseNode node)
    {
        node.DeleteNode();
        Nodes.Remove(node);

        if (AssetDatabase.Contains(node))
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node));
    }
    
    private void AddDisplacementToNodes()
    {
        foreach (var node in Nodes)
        {
            node.WindowRect.x += canvasDisplacement.x;
            node.WindowRect.y += canvasDisplacement.y;
        }
    }
    
    public void MoveToStart()
    {
        foreach (var node in Nodes)
        {
            if (node.GetType().Equals(typeof(StartNode)))
            {
                canvasDisplacement = _mousePos - new Vector2(node.WindowRect.x, node.WindowRect.y);
                AddDisplacementToNodes();
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
}
