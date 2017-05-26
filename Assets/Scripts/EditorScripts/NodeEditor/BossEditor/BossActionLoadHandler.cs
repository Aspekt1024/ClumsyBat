using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

public static class BossActionLoadHandler {

    public const string ResourcesDataFolder = "NPCs/Bosses/BossBehaviours/Data";
    public const string DataFolder = "Assets/Resources/NPCs/Bosses/BossBehaviours/Data";

    public static void Load (BehaviourSet behaviourSet) {

        string filePath = GetDataResourcePath(behaviourSet);
        XmlSerializer serializer = new XmlSerializer(typeof(ActionDataContainer), GetActionTypes());
        
        ActionDataContainer data;
        TextAsset actionDataText = (TextAsset)Resources.Load(filePath);
        using (var stream = new StringReader(actionDataText.text))
        {
            data = (ActionDataContainer)serializer.Deserialize(stream);
        }

        behaviourSet.Actions = data.Actions;

        foreach (var action in behaviourSet.Actions)
        {
            foreach (var conn in action.connections)
            {
                conn.Action = action;   // TODO Store this by creating INodeInterface : http://www.thomaslevesque.com/2009/06/12/c-parentchild-relationship-and-xml-serialization/
                if (conn.OtherActionID >= 0)
                {
                    conn.ConnectedInterface = GetConnection(behaviourSet.Actions, conn.OtherActionID, conn.OtherConnID);
                }
            }
        }
    }

    private static ActionConnection GetConnection(List<BaseAction> allActions, int actionID, int connID)
    {
        foreach (var action in allActions)
        {
            if (action.ID != actionID) continue;
            foreach (var conn in action.connections)
            {
                if (conn.ID != connID) continue;
                return conn;
            }
        }
        return null;
    }
    
    public static Type[] GetActionTypes()
    {
        List<Type> baseActionTypes = new List<Type>();
        foreach (Type t in Assembly.GetAssembly(typeof(BaseAction)).GetTypes().Where(type => type.IsSubclassOf(typeof(BaseAction))))
        {
            baseActionTypes.Add(t);
        }
        return baseActionTypes.ToArray();
    }
    
    private static string GetDataResourcePath(BehaviourSet behaviourSet)
    {
        string dataPath;
        if (behaviourSet.IsType<State>())
            dataPath = GetStateDataPath((State)behaviourSet);
        else
            dataPath = GetStateMachineDataPath((StateMachine)behaviourSet);

        return dataPath;
    }
    private static string GetStateMachineDataPath(StateMachine machine)
    {
        string stateMachineName = machine.Name;
        return string.Format("{0}/{1}/StateMachineRuntimeData", ResourcesDataFolder, stateMachineName);
    }
    
    private static string GetStateDataPath(State state)
    {
        string bossFolder = state.ParentMachine.Name;

        string bossDataPath = string.Format("{0}/{1}", ResourcesDataFolder, bossFolder);
        string stateFolder = state.Name.Replace(" ", "");
        
        return string.Format("{0}/{1}/RuntimeData", bossDataPath, stateFolder);
    }
}

[XmlRoot("ActionDataCollection")]
public class ActionDataContainer
{
    public List<BaseAction> Actions = new List<BaseAction>();
}
