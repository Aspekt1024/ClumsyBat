using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BossEditor : EditorWindow {

    public bool ConnectionMode;

    private List<BaseNode> _nodes = new List<BaseNode>();
    private BaseNode _currentNode;
    private Texture2D _bg;

    private BossEditorMouseInput _mouseInput;
    private BossNodeFactory _nodeFactory;
    
    private Vector2 _mousePos;

    [MenuItem("Window/Boss Editor")]
    private static void ShowEditor()
    {
        BossEditor editor = GetWindow<BossEditor>();
    }

    private void OnEnable()
    {
        _mouseInput = new BossEditorMouseInput(this);
        _nodeFactory = new BossNodeFactory(this);

        _bg = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        _bg.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.2f));
        _bg.Apply();
    }

    private void OnGUI()
    {
        Event e = Event.current;
        _mousePos = e.mousePosition;
        _mouseInput.ProcessMouseEvents(e);

        if (_bg != null)
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), _bg, ScaleMode.StretchToFill);

        DrawNodeCurves();
        DrawNodeWindows();
    }

    public BaseNode GetSelectedNode()
    {
        int index = -1;
        for (int i = 0; i < _nodes.Count; i++)
        {
            if (_nodes[i].WindowRect.Contains(_mousePos))
            {
                index = i;
                _currentNode = _nodes[i];
                break;
            }
        }
        return index >= 0 ? _nodes[index] : null;
    }

    private void DrawNodeWindow(int id)
    {
        _nodes[id].DrawWindow();
        if (!ConnectionMode)
        {
            GUI.DragWindow();
        }
    }
    
    private void DrawNodeWindows()
    {
        BeginWindows();
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].WindowRect = GUI.Window(i, _nodes[i].WindowRect, DrawNodeWindow, _nodes[i].WindowTitle);
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

        foreach (var node in _nodes)
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
        _nodes.Add(newNode);
    }

    public void RemoveNode(BaseNode node)
    {
        _nodes.Remove(node);
    }
}
