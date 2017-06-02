using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class NodeEditorSaveHandler {
    
    public static void Load(BaseEditor editor)
    {
        string filePath = GetStateMachineEditorDataPath(editor);
        if (editor.BehaviourSet.IsType<State>())
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

        serializer = new XmlSerializer(typeof(ActionDataContainer), BossActionLoadHandler.GetActionTypes());
        using (var stream = new FileStream(runtimeDataFilePath, FileMode.Create))
        {
            serializer.Serialize(stream, runtimeData);
        }

        AssetDatabase.ImportAsset(editorDataFilePath);
        AssetDatabase.ImportAsset(runtimeDataFilePath);
    }

    public static void CopyStateData(State fromState, State toState)
    {
        string fromEditorPath = GetStateEditorDataPath(fromState);
        string fromRuntimePath = GetStateRuntimeDataPath(fromState);
        string toEditorPath = GetStateEditorDataPath(toState);
        string toRuntimePath = GetStateRuntimeDataPath(toState);

        XmlSerializer editorDataSerializer = new XmlSerializer(typeof(NodeDataContainer), GetNodeTypes());
        XmlSerializer runtimeDataSerializer = new XmlSerializer(typeof(ActionDataContainer), BossActionLoadHandler.GetActionTypes());

        NodeDataContainer editorData = new NodeDataContainer();
        ActionDataContainer runtimeData = new ActionDataContainer();

        using (var stream = new FileStream(fromEditorPath, FileMode.Open))
        {
            editorData = (NodeDataContainer)editorDataSerializer.Deserialize(stream);
        }
        using (var stream = new FileStream(fromRuntimePath, FileMode.Open))
        {
            runtimeData = (ActionDataContainer)runtimeDataSerializer.Deserialize(stream);
        }

        using (var stream = new FileStream(toEditorPath, FileMode.Create))
        {
            editorDataSerializer.Serialize(stream, editorData);
        }
        using (var stream = new FileStream(toRuntimePath, FileMode.Create))
        {
            runtimeDataSerializer.Serialize(stream, runtimeData);
        }
    }
    
    private static ActionConnection ConvertInterfaceToAction(NodeInterface iface)
    {
        ActionConnection conn = new ActionConnection()
        {
            ID = iface.ID,
            OtherActionID = iface.ConnectedNodeID,
            OtherConnID = iface.ConnectedIfaceID,
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
    
    private static string GetEditorDataPath(BaseEditor editor)
    {
        string dataPath = GetStateMachineEditorDataPath(editor);
        if (editor.BehaviourSet.IsType<State>())
            dataPath = GetStateEditorDataPath(editor);
        
        return dataPath;
    }
    
    private static string GetStateMachineEditorDataPath(StateMachine machine)
    {
        string stateMachineName = machine.name;
        CreateFolderIfNotExists(BossActionLoadHandler.DataFolder, stateMachineName);
        return string.Format("{0}/{1}/StateMachineEditorData.xml", BossActionLoadHandler.DataFolder, stateMachineName);
    }

    private static string GetStateMachineEditorDataPath(BaseEditor editor)
    {
        string stateMachineName = GetBossName(editor);
        CreateFolderIfNotExists(BossActionLoadHandler.DataFolder, stateMachineName);
        return string.Format("{0}/{1}/StateMachineEditorData.xml", BossActionLoadHandler.DataFolder, stateMachineName);
    }

    private static string GetStateEditorDataPath(State state)
    {
        string bossFolder = state.ParentMachine.name;
        string stateName = state.name;
        return GetStateEditorDataPath(bossFolder, stateName);
    }

    private static string GetStateEditorDataPath(BaseEditor editor)
    {
        string bossFolder = GetBossName(editor);
        string stateName = editor.BehaviourSet.name;
        return GetStateEditorDataPath(bossFolder, stateName);
    }

    private static string GetStateEditorDataPath(string bossFolder, string stateName)
    {
        CreateFolderIfNotExists(BossActionLoadHandler.DataFolder, bossFolder);

        string bossDataPath = string.Format("{0}/{1}", BossActionLoadHandler.DataFolder, bossFolder);
        CreateFolderIfNotExists(bossDataPath, stateName);

        return string.Format("{0}/{1}/EditorData.xml", bossDataPath, stateName);
    }
    
    public static void CreateFolderIfNotExists(string path, string folderName)
    {
        if (AssetDatabase.IsValidFolder(path + "/" + folderName))
            return;

        AssetDatabase.CreateFolder(path, folderName);
    }

    private static State GetStateFromName(string stateName)
    {
        State[] allStates = Resources.LoadAll<State>(BossActionLoadHandler.ResourcesDataFolder);
        if (allStates.Length == 0) return null;

        foreach (var state in allStates)
        {
            if (state.name == stateName)
                return state;
        }
        return null;
    }

    public static string GetBossName(BaseEditor editor)
    {
        return editor.BehaviourSet.ParentMachine.name;
    }

    private static string GetRuntimeDataPath(BaseEditor editor)
    {
        string dataPath = GetStateMachineRuntimeDataPath(editor);
        if (editor.BehaviourSet.IsType<State>())
            dataPath = GetStateRuntimeDataPath(editor);

        return dataPath;
    }

    private static string GetStateMachineRuntimeDataPath(BaseEditor editor)
    {
        string bossFolder = GetBossName(editor);
        CreateFolderIfNotExists(BossActionLoadHandler.DataFolder, bossFolder);

        return string.Format("{0}/{1}/StateMachineRuntimeData.xml", BossActionLoadHandler.DataFolder, bossFolder);
    }

    private static string GetStateRuntimeDataPath(State state)
    {
        string bossFolder = state.ParentMachine.name;
        return GetStateRuntimeDataPath(bossFolder, state.name);
    }

    private static string GetStateRuntimeDataPath(BaseEditor editor)
    {
        string bossFolder = GetBossName(editor);
        string stateName = editor.BehaviourSet.name;
        return GetStateRuntimeDataPath(bossFolder, stateName);
    }

    private static string GetStateRuntimeDataPath(string bossFolder, string stateName)
    {
        CreateFolderIfNotExists(BossActionLoadHandler.DataFolder, bossFolder);

        string bossDataPath = string.Format("{0}/{1}", BossActionLoadHandler.DataFolder, bossFolder);
        CreateFolderIfNotExists(bossDataPath, stateName);

        return string.Format("{0}/{1}/RuntimeData.xml", bossDataPath, stateName);
    }
}

[XmlRoot("NodeDataCollection")]
public class NodeDataContainer
{
    public Vector2 EditorOffset = Vector2.zero;
    public List<BaseNode> Nodes = new List<BaseNode>();
}
