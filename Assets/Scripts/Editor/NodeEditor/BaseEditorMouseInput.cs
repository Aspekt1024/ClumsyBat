using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEditorMouseInput {

    protected enum MouseButtons
    {
        LeftClick = 0, RightClick = 1, MiddleClick = 2
    }

    public bool MouseDownHeld;

    private BaseEditor editor;
    private BaseContextMenus contextMenus;

    private BaseNode mouseDownNode;
    private int outputIndex;
    private BaseNode mouseUpNode;

    private Vector2 _mousePos;

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
        _mousePos = e.mousePosition;

        if (e.type == EventType.MouseDown)
        {
            if (ActionMouseDown(e.button))
            {
                e.Use();
            }
        }
        else if ((e.rawType == EventType.MouseUp && e.button == (int)MouseButtons.LeftClick)
            || e.type == EventType.MouseUp)
        {
            ActionMouseUp(e.button);
        }
        else if ((e.rawType == EventType.MouseDrag && e.button == (int)MouseButtons.LeftClick))
        {
            editor.Drag(e.delta);
        }
    }

    private bool ActionMouseDown(int button)
    {
        MouseDownHeld = true;
        mouseDownNode = editor.GetSelectedNode();
        bool clickActioned = false;
        switch (button)
        {
            case (int)MouseButtons.LeftClick:
                ActionLeftMouseDown();
                break;
            case (int)MouseButtons.RightClick:
                // do nothing
                clickActioned = true;
                break;
            case (int)MouseButtons.MiddleClick:
                // nothing?
                break;
        }
        return clickActioned;
    }

    private void ActionMouseUp(int button)
    {
        MouseDownHeld = false;
        mouseUpNode = editor.GetSelectedNode();
        switch (button)
        {
            case (int)MouseButtons.LeftClick:
                ActionLeftMouseUp();
                break;
            case (int)MouseButtons.RightClick:
                ActionRightMouseUp();
                break;
            case (int)MouseButtons.MiddleClick:
                // nothing?
                break;
        }
    }

    private void ActionLeftMouseDown()
    {
        if (mouseDownNode == null)
            editor.StartMovingEditorCanvas();
        else
        {
            if (!mouseDownNode.IsSelected && !Event.current.shift)
                editor.DeselectAllNodes();
            mouseDownNode.IsSelected = true;
            editor.Repaint();
            StartDraggingConnections();
        }
    }

    private void StartDraggingConnections()
    {
        outputIndex = mouseDownNode.OutputClicked(_mousePos);
        int inputIndex = mouseDownNode.InputClicked(_mousePos);
        if (outputIndex >= 0)
        {
            editor.ConnectionMode = true;   // TODO make this a property so deselect is done implicitly?
            editor.DeselectAllNodes();
        }
        else if (inputIndex >= 0)
        {
            MoveInputConnectionIfConnected(inputIndex);
        }
    }

    private void MoveInputConnectionIfConnected(int inputIndex)
    {
        if (mouseDownNode.InputIsConnected(inputIndex))
        {
            var output = mouseDownNode.GetConnectedInterfaceFromInput(inputIndex);
            mouseDownNode.DisconnectInput(inputIndex);

            mouseDownNode = output.connectedNode;
            outputIndex = output.connectedInterfaceIndex;
            mouseDownNode.DisconnectOutput(outputIndex);

            editor.SetSelectedNode(output.connectedNode);
            output.connectedNode.SetSelectedOutputPosFromIndex(output.connectedInterfaceIndex);
            editor.ConnectionMode = true;
            editor.DeselectAllNodes();
        }
    }

    private void ActionRightMouseUp()
    {
        if (mouseDownNode == null)
            contextMenus.ShowMenu();
        else
            contextMenus.ShowNodeMenu(mouseDownNode);
    }

    private void ActionLeftMouseUp()
    {
        MouseDownHeld = false;
        if (mouseUpNode != null)
        {
            int inputIndex = mouseUpNode.GetReleasedNode(_mousePos);
            if (editor.ConnectionMode && inputIndex >= 0)
            {
                mouseUpNode.ConnectInput(inputIndex, mouseDownNode, outputIndex);
            }
        }
        else
        {
            editor.DeselectAllNodes();
        }

        if (editor.MoveEditorMode)
            editor.StopMovingEditorCanvas();
        else
        {
            editor.ConnectionMode = false;
            editor.StopMovingNodes();
        }
    }
}
