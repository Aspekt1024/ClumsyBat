using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BossEditor : EditorWindow {

    public List<BaseNode> Nodes;
    public BossCreator BossCreatorObject;
    public bool ConnectionMode;

    private BaseNode _currentNode;
    private Texture2D _bg;
    private BossEditorMouseInput _mouseInput;
    private BossNodeFactory _nodeFactory;
    private Vector2 _mousePos;

    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    
    [MenuItem("Window/Boss Editor")]
    private static void ShowEditor()
    {
        BossEditor editor = GetWindow<BossEditor>();
        editor.BossCreatorObject = null;
        editor.stopwatch.Start();
    }

    private void OnEnable()
    {
        if (_mouseInput == null) _mouseInput = new BossEditorMouseInput(this);
        if (_nodeFactory == null) _nodeFactory = new BossNodeFactory(this);

        if (BossCreatorObject != null)
        {
            LoadBoss(BossCreatorObject);

            Debug.Log("load: Boss has " + BossCreatorObject.Nodes.Count + " nodes:");
            foreach (var node in BossCreatorObject.Nodes)
            {
                Debug.Log("load: " + node.WindowTitle + " : " + node.outputs.Count + " outputs, " + node.inputs.Count + " inputs.");
                foreach(var input in node.inputs)
                {
                    Debug.Log("load: interface index " + input.interfaceIndex);
                }
            }
        }
    }

    private void OnLostFocus()
    {
        if (BossCreatorObject != null)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                BaseNode node = Nodes[i];
                if (!AssetDatabase.Contains(node))
                {
                    AssetDatabase.AddObjectToAsset(node, BossCreatorObject);
                }
            }
            BossCreatorObject.Nodes = Nodes;
            EditorUtility.SetDirty(BossCreatorObject);

            Debug.Log("save: Boss has " + BossCreatorObject.Nodes.Count + " nodes:");
            foreach (var node in BossCreatorObject.Nodes)
            {
                Debug.Log("save: " + node.WindowTitle + " : " + node.outputs.Count + " outputs, " + node.inputs.Count + " inputs. w:" + node.WindowRect.width + " h:" + node.WindowRect.height);
            }
        }
    }

    private void Update()
    {
        // TODO remove this? just here in case we need a stopwatch. we currently don't so this does nothing.
        // Remove this if it's still here in april. 28.2.17
        if (BossCreatorObject == null) return;
        long dTime = stopwatch.ElapsedMilliseconds;
        float deltaTime = dTime / 1000f;
        
        foreach (BaseNode node in Nodes)
        {
            if (node != null)
            {
                node.Tick(deltaTime);
            }
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
        
        DrawNodeCurves();
        DrawNodeWindows();
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

    private void DrawNodeWindow(int id)
    {
        Nodes[id].DrawWindow();
        if (!ConnectionMode)
        {
            GUI.DragWindow();
        }
    }
    
    private void DrawNodeWindows()
    {
        BeginWindows();
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].WindowRect = GUI.Window(i, Nodes[i].WindowRect, DrawNodeWindow, Nodes[i].WindowTitle);
        }
        EndWindows();
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
        Nodes.Remove(node);
    }

    public void LoadBoss(BossCreator bossObj)
    {
        BossCreatorObject = bossObj;

        if (BossCreatorObject.Nodes == null || BossCreatorObject.Nodes.Count == 0)
            Nodes = new List<BaseNode>();
        else
            Nodes = BossCreatorObject.Nodes;
    }
}
