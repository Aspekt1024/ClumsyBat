using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class NodeEditor : EditorWindow {

    private List<BaseNode> windows = new List<BaseNode>();
    private Vector2 mousePos;
    private BaseNode selectedNode;
    private bool makeTransitionMode;

    [MenuItem("Window/Node Editor")]
    static void ShowEditor()
    {
        NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
    }

    void OnGUI()
    {
        Event e = Event.current;
        mousePos = e.mousePosition;

        // 1 = rightclick
        if (e.button == 1 && !makeTransitionMode)
        {
            if (e.type == EventType.mouseDown)
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                for (int i = 1; i< windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectIndex = 1;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (!clickedOnWindow)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add Input Node"), false, ContextCallback, "inputNode");
                    menu.AddItem(new GUIContent("Add Output Node"), false, ContextCallback, "outputNode");
                    menu.AddItem(new GUIContent("Add Calculation Node"), false, ContextCallback, "calcNode");
                    menu.AddItem(new GUIContent("Add Comparison Node"), false, ContextCallback, "compNode");

                    menu.ShowAsContext();
                    e.Use();
                }
                else
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Make Trainsition"), false, ContextCallback, "makeTransition");
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");

                    menu.ShowAsContext();
                    e.Use();
                }
            }
        }
        else if(e.button == 0 && e.type == EventType.MouseDown && makeTransitionMode)
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 1; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectIndex = 1;
                    clickedOnWindow = true;
                    break;
                }
            }

            if (clickedOnWindow && !windows[selectIndex].Equals(selectedNode))
            {
                windows[selectIndex].SetInput((BaseInputNode)selectedNode, mousePos);
                makeTransitionMode = false;
                selectedNode = null;
            }
            if (!clickedOnWindow)
            {
                makeTransitionMode = false;
                selectedNode = null;
            }

            e.Use();
        }
        else if (e.button == 0 && e.type == EventType.MouseDown && !makeTransitionMode)
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 1; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectIndex = 1;
                    clickedOnWindow = true;
                    break;
                }
            }

            if (clickedOnWindow)
            {
                BaseInputNode nodeToChange = windows[selectIndex].ClickedOnInput(mousePos);

                if (nodeToChange != null)
                {
                    selectedNode = nodeToChange;
                    makeTransitionMode = true;
                }
            }
        }

        if (makeTransitionMode && selectedNode != null)
        {
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);
            DrawNodeCurve(selectedNode.windowRect, mouseRect);
            Repaint();
        }

        foreach (BaseNode n in windows)
        {
            n.DrawCurves();
        }

        BeginWindows();

        for (int i = 0; i < windows.Count; i++)
        {
            windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeCurve, windows[i].windowTitle);
        }

        EndWindows();
    }

    private void DrawNodeCurve(int id)
    {
        throw new NotImplementedException();
    }

    void ContextCallback(object obj)
    {

    }

    public static void DrawNodeCurve(Rect start, Rect end)
    {

    }
}
