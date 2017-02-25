using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BossEditorMouseInput {

    private enum MouseButtons
    {
        LeftClick = 0, RightClick = 1, MiddleClick = 2
    }

    // TODO auto search nodetypes based on inheritance tree?
    public enum NodeTypes
    {
        Start,
        End,
        SaySomething,
        Jump,
        Die
    }
    
    public enum NodeMenuSelections
    {
        DoNothing,
        DeleteNode
    }

    private BossEditor _editor;

    private BaseNode _selectedNode;
    private BaseNode _releasedNode;

    private Vector2 _mousePos;
    
    public BossEditorMouseInput(BossEditor editor)
    {
        _editor = editor;
    }

	public void ProcessMouseEvents(Event e)
    {
        _mousePos = e.mousePosition;

        // TODO move this to separate class?
        if (e.type == EventType.MouseDown)
        {
            if (ActionMouseDown(e.button))
            {
                e.Use();
            }
        }
        else if (e.type == EventType.MouseUp)
        {
            ActionMouseUp(e.button);
        }

    }
    
    private bool ActionMouseDown(int button)
    {
        _selectedNode = _editor.GetSelectedNode();
        bool clickActioned = false;
        switch (button)
        {
            case (int)MouseButtons.LeftClick:
                clickActioned = ActionLeftMouseDown();
                break;
            case (int)MouseButtons.RightClick:
                ActionRightMouseDown();
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
        _releasedNode = _editor.GetSelectedNode();
        switch (button)
        {
            case (int)MouseButtons.LeftClick:
                ActionLeftMouseUp();
                break;
            case (int)MouseButtons.RightClick:
                // no action on mouseup rightclick
                break;
            case (int)MouseButtons.MiddleClick:
                // nothing?
                break;
        }
    }

    private bool ActionLeftMouseDown()
    {
        if (_selectedNode == null) return false;
        if (_selectedNode.OutputClicked(_mousePos)) {
            _editor.ConnectionMode = true;
        }
        return false;
    }

    private void ActionRightMouseDown()
    {
        if (_selectedNode == null)
        {
            BossEditorContextMenus.ShowMenu(ContextCallback);
        }
        else
        {
            BossEditorContextMenus.ShowNodeMenu(ContextCallback);
        }
    }

    private void ActionLeftMouseUp()
    {
        if (_releasedNode != null)
        {
            if (_editor.ConnectionMode && _releasedNode.ReleasedOnInput(_mousePos))
            {
                _releasedNode.SetInput(_selectedNode);
            }
        }
        _editor.ConnectionMode = false;
    }
    
    public void ContextCallback(object obj)
    {
        if (obj.GetType().Equals(typeof(NodeTypes)))
        {
            BossEditorEvents.CreateNode((NodeTypes)obj);
        }
        else if (obj.GetType().Equals(typeof(NodeMenuSelections)))
        {
            ActionNodeMenuSelection((NodeMenuSelections)obj);
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
                _editor.RemoveNode(_selectedNode);
                break;
        }
    }
}
