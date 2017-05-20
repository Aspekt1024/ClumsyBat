using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class NodeEditorSaveHandler {
    
    private const string dataFolder = "Assets/Resources/NPCs/Bosses/BossBehaviours/Data";
    
    public static void Load(BaseEditor editor)
    {
        LoadNodeData(editor);
        return;



        // TODO move this to separate action loader
        string filePath = string.Format("{0}/{1}.xml", dataFolder, "testNodeData");
        XmlSerializer serializer = new XmlSerializer(typeof(NodeDataContainer), GetActionTypes());


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
                iface.Node = node;   // TODO Store this by creating INodeInterface : http://www.thomaslevesque.com/2009/06/12/c-parentchild-relationship-and-xml-serialization/
                if (iface.ConnectedNodeID >= 0)
                {
                    iface.ConnectedInterface = GetInterface(editor.Nodes, iface.ConnectedNodeID, iface.ConnectedIfaceID);
                }
            }
        }
    }
    
    private static void LoadNodeData(BaseEditor editor)
    {
        string filePath = string.Format("{0}/{1}.xml", dataFolder, "testNodeData");
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
                iface.Node = node;   // TODO Store this by creating INodeInterface : http://www.thomaslevesque.com/2009/06/12/c-parentchild-relationship-and-xml-serialization/
                if (iface.ConnectedNodeID >= 0)
                {
                    iface.ConnectedInterface = GetInterface(editor.Nodes, iface.ConnectedNodeID, iface.ConnectedIfaceID);
                }
            }
        }
    }

    private static NodeInterface GetInterface(List<BaseNode> allNodes, int nodeID, int ifaceID)
    {
        foreach (var node in allNodes)
        {
            if (node.ID != nodeID) continue;
            foreach (var iface in node.interfaces)
            {
                if (iface.ID != ifaceID) continue;
                return iface;
            }
        }
        return null;
    }

    public static void Save(BaseEditor editor)
    {
        string editorDataFilePath = string.Format("{0}/{1}.xml", dataFolder, "testNodeData");
        string runtimeDataFilePath = string.Format("{0}/{1}.xml", dataFolder, "testRuntimeData");

        NodeDataContainer editorData = new NodeDataContainer();
        ActionDataContainer runtimeData = new ActionDataContainer();
        editorData.Nodes = editor.Nodes;
        editorData.EditorOffset = editor.CanvasOffset;

        runtimeData.Actions = new List<BaseAction>();
        foreach (var node in editorData.Nodes)
        {
            BaseAction newAction = node.GetAction();
            newAction.ID = node.ID;
            newAction.connections = new List<ActionConnection>();

            foreach (var iface in node.interfaces)
            {
                if (iface.ConnectedInterface == null)
                {
                    iface.ConnectedNodeID = -1;
                    iface.ConnectedIfaceID = -1;
                }
                else
                {
                    iface.ConnectedNodeID = iface.ConnectedInterface.Node.ID;
                    iface.ConnectedIfaceID = iface.ConnectedInterface.ID;
                }
                newAction.connections.Add(ConvertInterfaceToAction(iface));
            }
            runtimeData.Actions.Add(newAction);
        }

        XmlSerializer serializer = new XmlSerializer(typeof(NodeDataContainer), GetNodeTypes());
        using (var stream = new FileStream(editorDataFilePath, FileMode.Create))
        {
            serializer.Serialize(stream, editorData);
        }

        serializer = new XmlSerializer(typeof(ActionDataContainer), GetActionTypes());
        using (var stream = new FileStream(runtimeDataFilePath, FileMode.Create))
        {
            serializer.Serialize(stream, runtimeData);
        }

        AssetDatabase.ImportAsset(editorDataFilePath);
        AssetDatabase.ImportAsset(runtimeDataFilePath);
    }

    private static ActionConnection ConvertInterfaceToAction(NodeInterface iface)
    {
        ActionConnection conn = new ActionConnection()
        {
            ID = iface.ID,
            OtherActionID = iface.ConnectedIfaceID,
            OtherConnID = iface.ConnectedNodeID,
            Direction = iface.Direction
        };
        return conn;
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

    private static Type[] GetActionTypes()
    {
        List<Type> baseActionTypes = new List<Type>();
        foreach (Type t in Assembly.GetAssembly(typeof(BaseAction)).GetTypes().Where(type => type.IsSubclassOf(typeof(BaseAction))))
        {
            baseActionTypes.Add(t);
        }
        return baseActionTypes.ToArray();
    }
}

[XmlRoot("NodeDataCollection")]
public class NodeDataContainer
{
    public Vector2 EditorOffset = Vector2.zero;
    public List<BaseNode> Nodes = new List<BaseNode>();
}

[XmlRoot("ActionDataCollection")]
public class ActionDataContainer
{
    public List<BaseAction> Actions = new List<BaseAction>();
}