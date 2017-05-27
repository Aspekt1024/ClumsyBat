using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeRuntimeBorders {

    private BaseEditor editor;
    private BaseNode node;
    private BaseAction action;

    private float animTimer;
    private const float animDuration = 0.8f;

    public NodeRuntimeBorders(BaseNode thisNode)
    {
        node = thisNode;
        editor = thisNode.ParentEditor;
    }

    public void Update()
    {
        if (action == null) FindActionID();
        if (action == null) return;

        if (action.IsActive || action.IsNewActivation)
        {
            action.IsNewActivation = false;
            animTimer = 0f;
            node.SelectedBorderAlpha = 1f;
            return;
        }
        else if (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            node.SelectedBorderAlpha = (1f - animTimer / animDuration);
            return;
        }

        node.SelectedBorderAlpha = -1f;
    }

    private void FindActionID()
    {
        for (int i = 0; i < editor.BehaviourSet.Actions.Count; i++)
        {
            if (node.ID != editor.BehaviourSet.Actions[i].ID) continue;

            action = editor.BehaviourSet.Actions[i];
            return;
        }
    }
}

