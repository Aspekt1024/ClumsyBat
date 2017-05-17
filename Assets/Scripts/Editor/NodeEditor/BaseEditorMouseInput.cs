using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEditorMouseInput {

    private enum MouseButtons
    {
        LeftClick = 0, RightClick = 1, MiddleClick = 2
    }

    private BaseEditor editor;
    private BaseContextMenus contextMenus;

    private BaseNode mouseDownNode;
    private BaseNode mouseUpNode;

    private bool isDragged;
    private bool canvasWasDragged;

    public BaseEditorMouseInput(BaseEditor editorRef)
    {
        editor = editorRef;
        if (editor.GetType().Equals(typeof(BossStateEditor)))
        {
            contextMenus = new BossStateEditorContextMenus(editor);
        }
        else if (editor.GetType().Equals(typeof(BossEditor)))
        {
            contextMenus = new BossEditorContextMenus(editor);
        }
    }

    public void ProcessMouseEvents(Event e)
    {
        if(editor.NodeData == null) Debug.Log("process");

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == (int)MouseButtons.LeftClick)
                    ActionLeftMouseDown();
                break;

            case EventType.MouseUp:
                isDragged = false;
                if (e.button == (int)MouseButtons.LeftClick)
                    ActionLeftMouseUp();
                else if (e.button == (int)MouseButtons.RightClick)
                    ActionRightMouseUp();
                break;

            case EventType.MouseDrag:
                if (isDragged && e.button == (int)MouseButtons.LeftClick)
                {
                    canvasWasDragged = true;
                    editor.Drag(e.delta);
                    e.Use();
                }
                break;
        }
    }

    private void ActionLeftMouseDown()
    {
        mouseDownNode = editor.GetSelectedNode();
        if (mouseDownNode == null)
        {
            isDragged = true;
            editor.StartMovingEditorCanvas();
        }
    }
    
    private void ActionLeftMouseUp()
    {
        editor.DeselectAllNodes();
        editor.StopMovingEditorCanvas();
    }

    private void ActionRightMouseUp()
    {
        mouseUpNode = editor.GetSelectedNode();
        if (mouseUpNode == null)
            contextMenus.ShowMenu();
        else
            contextMenus.ShowNodeMenu(mouseUpNode);
    }

}
