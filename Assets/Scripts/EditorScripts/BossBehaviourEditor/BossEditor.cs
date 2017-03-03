using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BossEditor : EditorWindow {

    public List<BaseNode> Nodes;
    public BossCreator BossCreatorObject;
    public bool ConnectionMode;
    public bool MoveEditorMode;

    private BaseNode _currentNode;
    private Texture2D _bg;
    private BossEditorMouseInput _mouseInput;
    private Vector2 _mousePos;
    private Vector2 startMousePos;
    private Vector2 canvasDisplacement;
    private float timeSinceUpdate;

    [MenuItem("Window/Boss Editor")]
    private static void ShowEditor()
    {
        BossEditor editor = GetWindow<BossEditor>();
        editor.BossCreatorObject = null;
    }

    private void OnEnable()
    {
        if (_mouseInput == null) _mouseInput = new BossEditorMouseInput(this);

        if (BossCreatorObject != null) {
            LoadBoss(BossCreatorObject);
        }
    }

    private void OnLostFocus()
    {
        if (BossCreatorObject != null)
        {
            // TODO make this... nicer...
            string assetFolder = AssetDatabase.GetAssetPath(BossCreatorObject);
            assetFolder = assetFolder.Substring(0, assetFolder.Length - ("/" + BossCreatorObject.name + ".asset").Length);
            string folderName = BossCreatorObject.name + "Nodes";
            if (!AssetDatabase.IsValidFolder(assetFolder + "/" + folderName))
                AssetDatabase.CreateFolder(assetFolder, folderName);
            assetFolder += "/" + folderName + "/";
            for (int i = 0; i < Nodes.Count; i++)
            {
                BaseNode node = Nodes[i];
                if (!AssetDatabase.Contains(node))
                {
                    int assetNum = 1;
                    while (AssetDatabase.LoadAssetAtPath<BaseNode>(assetFolder + node.WindowTitle + assetNum + ".asset") != null)
                        assetNum++;

                    AssetDatabase.CreateAsset(node, assetFolder + node.WindowTitle + assetNum + ".asset");
                }

                EditorUtility.SetDirty(node); 
            }
            BossCreatorObject.Nodes = Nodes;
            EditorUtility.SetDirty(BossCreatorObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
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
        // TODO if no object, show text to display one, else show title in editor of what the boss is
        if (BossCreatorObject == null) return;

        Event e = Event.current;
        _mousePos = e.mousePosition;
        
        _mouseInput.ProcessMouseEvents(e);

        if (_bg == null)
        {
            _bg = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _bg.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.2f));
            _bg.Apply();
        }
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), _bg, ScaleMode.StretchToFill);
        
        DrawNodeWindows();
        DrawNodeCurves();
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
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0.7f, 0.7f, 1f, 0.06f);

        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 7);
        }
        Handles.DrawBezier(startPos, endPos, startTan, endTan, new Color(0.7f, 0.7f, 1f), null, 3);
    }

    public void AddNode(BaseNode newNode)
    {
        newNode.SetWindowRect(_mousePos);
        newNode.SetupNode();
        Nodes.Add(newNode);
    }

    public void RemoveNode(BaseNode node)
    {
        node.DeleteNode();
        Nodes.Remove(node);

        if(AssetDatabase.Contains(node))
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node));
    }

    public void LoadBoss(BossCreator bossObj)
    {
        BossCreatorObject = bossObj;

        if (BossCreatorObject.Nodes == null || BossCreatorObject.Nodes.Count == 0)
            Nodes = new List<BaseNode>();
        else
            Nodes = BossCreatorObject.Nodes;
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

    private void AddDisplacementToNodes()
    {
        foreach(var node in Nodes)
        {
            node.WindowRect.x += canvasDisplacement.x;
            node.WindowRect.y += canvasDisplacement.y;
        }
    }
}
