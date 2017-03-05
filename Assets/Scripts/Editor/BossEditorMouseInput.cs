using UnityEngine;

public class BossEditorMouseInput {

    private enum MouseButtons
    {
        LeftClick = 0, RightClick = 1, MiddleClick = 2
    }

    public bool MouseDownHeld;

    private BaseEditor _editor;
    private BaseContextMenus contextMenus;

    private BaseNode _mouseDownNode;
    private int _outputIndex;
    private BaseNode _mouseUpNode;

    private Vector2 _mousePos;
    
    public BossEditorMouseInput(BaseEditor editor)
    {
        _editor = editor;
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

    }
    
    private bool ActionMouseDown(int button)
    {
        MouseDownHeld = true;
        _mouseDownNode = _editor.GetSelectedNode();
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
        _mouseUpNode = _editor.GetSelectedNode();
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
        if (_mouseDownNode == null)
            _editor.StartMovingEditorCanvas();
        else
            StartDraggingConnections();
    }

    private void StartDraggingConnections()
    {
        _outputIndex = _mouseDownNode.OutputClicked(_mousePos);
        int inputIndex = _mouseDownNode.InputClicked(_mousePos);
        if (_outputIndex >= 0)
        {
            _editor.ConnectionMode = true;
        }
        else if (inputIndex >= 0)
        {
            MoveInputConnectionIfConnected(inputIndex);
        }
    }

    private void MoveInputConnectionIfConnected(int inputIndex)
    {
        if (_mouseDownNode.InputIsConnected(inputIndex))
        {
            var output = _mouseDownNode.GetConnectedInterfaceFromInput(inputIndex);
            _mouseDownNode.DisconnectInput(inputIndex);

            _mouseDownNode = output.connectedNode;
            _outputIndex = output.connectedInterfaceIndex;
            _mouseDownNode.DisconnectOutput(_outputIndex);

            _editor.SetSelectedNode(output.connectedNode);
            _editor.ConnectionMode = true;
        }
    }

    private void ActionRightMouseUp()
    {
        if (_mouseDownNode == null)
            contextMenus.ShowMenu();
        else
            contextMenus.ShowNodeMenu(_mouseDownNode);
    }

    private void ActionLeftMouseUp()
    {
        if (_mouseUpNode != null)
        {
            int inputIndex = _mouseUpNode.GetReleasedNode(_mousePos);
            if (_editor.ConnectionMode && inputIndex >= 0)
            {
                _mouseUpNode.SetInput(inputIndex, _mouseDownNode, _outputIndex);
            }
        }
        if (_editor.MoveEditorMode)
            _editor.StopMovingEditorCanvas();
        else
            _editor.ConnectionMode = false;
    }
}
