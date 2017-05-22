using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeRuntimeBorders {

    private BaseEditor editor;
    private List<NodeActiveBorder> activeBorders = new List<NodeActiveBorder>();

    public NodeRuntimeBorders(BaseEditor parentEditor)
    {
        editor = parentEditor;
    }

    public void Update()
    {
        FindActiveNode();

        foreach (var border in activeBorders)
        {
            if (!border.Update())
                border.Active = false;
        }

        // TODO remove inactive borders
    }

    private void FindActiveNode()
    {
        foreach (var action in editor.StateMachine.Actions)
        {
            foreach (var node in editor.Nodes)
            {
                if (node.ID != action.ID || !action.NewActivation) continue;

                ActivateBorder(node, action);
            }
        }
    }

    private void ActivateBorder(BaseNode node, BaseAction action)
    {
        action.NewActivation = false;

        foreach (var border in activeBorders)
        {
            if (border.Node == node)
            {
                border.Active = true;
                border.AnimTimer = 0f;
                return;
            }
        }

        NodeActiveBorder newBorder = new NodeActiveBorder(node, action);
        activeBorders.Add(newBorder);
    }
}

public class NodeActiveBorder
{
    public bool Active;
    public BaseNode Node;
    public float AnimTimer;

    private BaseAction action;
    private const float animDuration = 0.8f;

    public NodeActiveBorder(BaseNode nodeRef, BaseAction actionRef)
    {
        Node = nodeRef;
        action = actionRef;
        AnimTimer = 0f;
    }

    public bool Update()
    {
        if (action.Active)
        {
            Node.SelectedBorderAlpha = 1f;
            return true;
        }
        else if (AnimTimer < animDuration)
        {
            AnimTimer += Time.deltaTime;
            Node.SelectedBorderAlpha = (1f - AnimTimer / animDuration);
            return true;
        }

        Node.SelectedBorderAlpha = -1f;
        return false;
    }
}
