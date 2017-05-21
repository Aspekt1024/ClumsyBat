using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class NodeEditorSaveHandler {
    
    public const string DataFolder = "Assets/Resources/NPCs/Bosses/BossBehaviours/Data";
    
    public static void Load(BaseEditor editor)
    {
        LoadNodeData(editor);
        return;
        



        // TODO move this to separate action loader
        string filePath = string.Format("{0}/{1}.xml", DataFolder, "testNodeData");
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
        string filePath = GetStateMachineEditorDataPath(editor);
        if (editor.StateMachine.IsType<BossState>())
            filePath = GetStateEditorDataPath(editor);

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
            
            if (node.IsType<StateNode>())
            {
                ((StateNode)node).State = GetStateFromName(((StateNode)node).StateName);
            }

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
        string editorDataFilePath = GetEditorDataPath(editor);
        string runtimeDataFilePath = GetRuntimeDataPath(editor);

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

    private static string GetEditorDataPath(BaseEditor editor)
    {
        string dataPath = GetStateMachineEditorDataPath(editor);
        if (editor.StateMachine.IsType<BossState>())
            dataPath = GetStateEditorDataPath(editor);

        return dataPath;
    }

    private static string GetRuntimeDataPath(BaseEditor editor)
    {
        string dataPath = GetStateMachineRuntimeDataPath(editor);
        if (editor.StateMachine.IsType<BossState>())
            dataPath = GetStateRuntimeDataPath(editor);

        return dataPath;
    }

    private static string GetStateMachineEditorDataPath(BaseEditor editor)
    {
        string stateMachineName = editor.StateMachine.name;
        CreateFolderIfNotExists(stateMachineName);
        return string.Format("{0}/{1}/StateMachineEditorData.xml", DataFolder, stateMachineName);
    }

    private static string GetStateMachineRuntimeDataPath(BaseEditor editor)
    {
        string stateMachineName = editor.StateMachine.name;
        CreateFolderIfNotExists(stateMachineName);
        return string.Format("{0}/{1}/StateMachineRuntimeData.xml", DataFolder, stateMachineName);
    }

    private static string GetStateEditorDataPath(BaseEditor editor)
    {
        string stateName = editor.StateMachine.name;
        string stateMachineFolder = editor.StateMachine.RootStateMachine.name;
        CreateFolderIfNotExists(stateMachineFolder);
        return string.Format("{0}/{1}/{2}EditorData.xml", DataFolder, stateMachineFolder, stateName);
    }

    private static string GetStateRuntimeDataPath(BaseEditor editor)
    {
        string stateName = editor.StateMachine.name;
        string stateMachineFolder = editor.StateMachine.RootStateMachine.name;
        CreateFolderIfNotExists(stateMachineFolder);
        return string.Format("{0}/{1}/{2}RuntimeData.xml", DataFolder, stateMachineFolder, stateName);
    }

    public static void CreateFolderIfNotExists(string folderName)
    {
        if (AssetDatabase.IsValidFolder(DataFolder + "/" + folderName))
            return;

        AssetDatabase.CreateFolder(DataFolder, folderName);
    }

    private static BossState GetStateFromName(string stateName)
    {
        stateName = stateName.Replace(" ", "");
        BossState[] allStates = Resources.LoadAll<BossState>("NPCs/Bosses/BossBehaviours");
        if (allStates.Length == 0) return null;

        foreach (var state in allStates)
        {
            if (state.name == stateName)
                return state;
        }
        return null;
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