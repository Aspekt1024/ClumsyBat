using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Used to persist node data for the Boss Editor
/// </summary>
public class BossEditorNodeData : ScriptableObject {

    public List<BaseNode> Nodes = new List<BaseNode>();

    public StartNode GetStartNode()
    {
        foreach (var node in Nodes)
        {
            if (node.IsType<StartNode>())
                return (StartNode)node;
        }
        return null;
    }

    public void DeleteAllNodes()
    {
        foreach (var node in Nodes)
        {
            node.DeleteNode();
            Nodes.Remove(node);

            if (AssetDatabase.Contains(node))
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node));
        }
    }
}
