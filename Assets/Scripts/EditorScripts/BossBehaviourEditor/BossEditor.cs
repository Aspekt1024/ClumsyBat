using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BossEditor : EditorWindow {

    private List<BaseNode> nodes = new List<BaseNode>();
    private BaseNode selectedNode;
    private Vector2 mousePos;
    private bool bConnectionMode;
    private static Texture2D bg;

    private enum MouseButtons
    {
        LeftClick = 0, RightClick = 1, MiddleClick = 2
    }

    // TODO auto search nodetypes based on inheritance tree?
    public enum NodeTypes
    {
        SaySomething,
        Jump,
        Die
    }

    public enum NodeMenuSelections
    {
        DoNothing,
        DeleteNode
    }

    [MenuItem("Window/Boss Editor")]
    private static void ShowEditor()
    {
        BossEditor editor = GetWindow<BossEditor>();
    }

    private void OnEnable()
    {
        bg = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        bg.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.2f));
        bg.Apply();
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), bg, ScaleMode.StretchToFill);

        Event e = Event.current;
        mousePos = e.mousePosition;

        if (e.button == (int)MouseButtons.RightClick && e.type == EventType.mouseDown)
        {
            ActionRightClick();
            e.Use();
        }

        BeginWindows();
        
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].WindowRect = GUI.Window(i, nodes[i].WindowRect, DrawNodeWindow, nodes[i].WindowTitle);
        }
        EndWindows();
    }
    
    private void ActionRightClick()
    {
        int selectedIndex = GetSelectedIndex();
        bool clickedOnWindow = selectedIndex >= 0;

        if (!clickedOnWindow) {
            ShowContextMenu();
        } else {
            selectedNode = nodes[selectedIndex];
            ShowNodeContextMenu();
        }
    }

    private int GetSelectedIndex()
    {
        int index = -1;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].WindowRect.Contains(mousePos))
            {
                index = i;
                break;
            }
        }
        return index;
    }

    private void ShowContextMenu()
    {
        GenericMenu menu = BossEditorContextMenu.GetMenu(ContextCallback);
        menu.ShowAsContext();
    }

    private void ShowNodeContextMenu()
    {
        GenericMenu menu = BossEditorContextMenu.GetNodeMenu(ContextCallback);
        menu.ShowAsContext();
    }

    private void DrawNodeWindow(int id)
    {
        nodes[id].DrawWindow();
        GUI.DragWindow();
    }

    public void ContextCallback(object obj)
    {
        if (obj.GetType().Equals(typeof(NodeTypes)))
        {
            CreateNode((NodeTypes)obj);
        }
        else if (obj.GetType().Equals(typeof(NodeMenuSelections)))
        {
            ActionNodeMenuSelection((NodeMenuSelections)obj);
        }

    }

    private void CreateNode(NodeTypes node)
    {
        BaseNode inputNode = null;
        switch (node)
        {
            case NodeTypes.SaySomething:
                inputNode = CreateInstance<SpeechNode>();
                break;
            case NodeTypes.Jump:
                inputNode = CreateInstance<BaseNode>();
                break;
            case NodeTypes.Die:
                inputNode = CreateInstance<BaseNode>();
                break;
        }
        if (inputNode != null)
        {
            inputNode.SetWindowRect(mousePos);
            nodes.Add(inputNode);
        }
    }

    private void ActionNodeMenuSelection(NodeMenuSelections selection)
    {
        switch (selection)
        {
            case NodeMenuSelections.DoNothing:
                // as it says
                break;
            case NodeMenuSelections.DeleteNode:
                nodes.Remove(selectedNode);
                break;
        }
    }
}
