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
            border.Update();
        }
    }

    private void FindActiveNode()
    {
        foreach (var action in editor.BehaviourSet.Actions)
        {
            foreach (var node in editor.Nodes)
            {
                if (node.ID != action.ID || !(action.IsNewActivation || action.IsActive)) continue;
                ActivateBorder(node, action);
            }
        }
    }

    private void ActivateBorder(BaseNode node, BaseAction action)
    {
        foreach (var border in activeBorders)
        {
            if (border.Node == node)
                return;
        }

        NodeActiveBorder newBorder = new NodeActiveBorder(node, action);
        activeBorders.Add(newBorder);
    }
}

public class NodeActiveBorder
{
    public BaseNode Node;

    private float animTimer;
    private BaseAction action;
    private const float animDuration = 0.8f;

    public NodeActiveBorder(BaseNode nodeRef, BaseAction actionRef)
    {
        Node = nodeRef;
        action = actionRef;
        animTimer = 0f;
    }

    public void Update()
    {
        if (action.IsActive || action.IsNewActivation)
        {
            action.IsNewActivation = false;
            animTimer = 0f;
            Node.SelectedBorderAlpha = 1f;
            return;
        }
        else if (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            Node.SelectedBorderAlpha = (1f - animTimer / animDuration);
            return;
        }

        Node.SelectedBorderAlpha = -1f;
    }
}
