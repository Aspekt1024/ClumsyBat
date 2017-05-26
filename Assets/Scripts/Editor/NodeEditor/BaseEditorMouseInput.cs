using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEditorMouseInput {

    public bool CanvasWasDragged;

    private enum MouseButtons
    {
        LeftClick = 0, RightClick = 1, MiddleClick = 2
    }

    private BaseEditor editor;
    private BaseContextMenus contextMenus;

    private BaseNode mouseDownNode;
    private BaseNode mouseUpNode;

    private bool isDragged;

    public BaseEditorMouseInput(BaseEditor editorRef)
    {
        editor = editorRef;
        contextMenus = new BossEditorContextMenus(editor);
    }

    public void ProcessMouseEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == (int)MouseButtons.LeftClick)
                    ActionLeftMouseDown();
                break;

            case EventType.MouseUp:
                if (e.button == (int)MouseButtons.RightClick)
                    ActionRightMouseUp();
                break;

            case EventType.MouseDrag:
                if (isDragged && e.button == (int)MouseButtons.LeftClick)
                {
                    editor.Drag(e.delta);
                    e.Use();
                }
                break;
        }
        if (e.rawType == EventType.mouseUp && e.button == (int)MouseButtons.LeftClick)
        {
            isDragged = false;
            if (e.button == (int)MouseButtons.LeftClick)
                ActionLeftMouseUp();
        }
    }

    private void ActionLeftMouseDown()
    {
        mouseDownNode = editor.GetSelectedNode();
        isDragged = mouseDownNode == null;
    }
    
    private void ActionLeftMouseUp()
    {
        if (!CanvasWasDragged)
            editor.DeselectAllNodes();

        CanvasWasDragged = false;
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
