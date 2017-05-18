using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.Reflection;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public static class NodeEditorSaveHandler {
    
    private const string dataFolder = "Assets/Resources/NPCs/Bosses/BossBehaviours/Data";
    
    public static void Load(BaseEditor editor)
    {
        string filePath = string.Format("{0}/{1}.xml", dataFolder, "test");
        XmlSerializer serializer = new XmlSerializer(typeof(NodeDataContainer), GetNodeTypes());
        
        NodeDataContainer data;
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            data = (NodeDataContainer)serializer.Deserialize(stream);
        }

        editor.Nodes = data.Nodes;
        editor.CanvasOffset = data.EditorOffset;
        
        foreach (var node in editor.Nodes)
        {
            node.ParentEditor = editor;
            node.Transform = new NodeTransform(node);
            foreach (var iface in node.interfaces)
            {
                iface.Node = node;
            }
        }
    }

    public static void Save(BaseEditor editor)
    {
        string filePath = string.Format("{0}/{1}.xml", dataFolder, "test");

        NodeDataContainer data = new NodeDataContainer();
        data.Nodes = editor.Nodes;
        data.EditorOffset = editor.CanvasOffset;
        
        XmlSerializer serializer = new XmlSerializer(typeof(NodeDataContainer), GetNodeTypes());
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, data);
        }
    }

    private static Type[] GetNodeTypes()
    {
        List<Type> baseNodeTypes = new List<Type>();
        foreach (Type t in Assembly.GetAssembly(typeof(BaseNode)).GetTypes().Where(type => type.IsSubclassOf(typeof(BaseNode))))
        {
            baseNodeTypes.Add(t);
        }
        return baseNodeTypes.ToArray();

    }
}

[XmlRoot("NodeDataCollection")]
public class NodeDataContainer
{
    public Vector2 EditorOffset = Vector2.zero;
    public List<BaseNode> Nodes = new List<BaseNode>();
}