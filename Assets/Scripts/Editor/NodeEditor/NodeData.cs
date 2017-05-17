using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Used to persist node data for the Node Editor
/// </summary>
public class NodeData {

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

            // TODO update asset database
            //if (AssetDatabase.Contains(node))
            //    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node));
        }
    }

    public void Save()
    {

    }
}
