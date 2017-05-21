using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

public static class BossActionLoadHandler {

    public const string DataFolder = "Assets/Resources/NPCs/Bosses/BossBehaviours/Data";

    public static void Load (BossStateMachine bossStateMachine) {

        string filePath = GetDataPath(bossStateMachine);
        XmlSerializer serializer = new XmlSerializer(typeof(ActionDataContainer), GetActionTypes());
        
        ActionDataContainer data;
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            data = (ActionDataContainer)serializer.Deserialize(stream);
        }

        bossStateMachine.Actions = data.Actions;

        foreach (var action in bossStateMachine.Actions)
        {
            foreach (var conn in action.connections)
            {
                conn.Action = action;   // TODO Store this by creating INodeInterface : http://www.thomaslevesque.com/2009/06/12/c-parentchild-relationship-and-xml-serialization/
                if (conn.OtherActionID >= 0)
                {
                    conn.ConnectedInterface = GetConnection(bossStateMachine.Actions, conn.OtherActionID, conn.OtherConnID);
                    Debug.Log("found that " + action + " " + conn.Direction + " is connected to " + conn.ConnectedInterface.Action + " " + conn.ConnectedInterface.Direction);
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
    
    private static string GetDataPath(StateMachine stateMachine)
    {
        string dataPath = GetStateMachineDataPath(stateMachine);
        if (stateMachine.IsType<BossState>())
            dataPath = GetStateDataPath(stateMachine);

        return dataPath;
    }
    private static string GetStateMachineDataPath(StateMachine stateMachine)
    {
        string stateMachineName = stateMachine.name;
        return string.Format("{0}/{1}/StateMachineRuntimeData.xml", DataFolder, stateMachineName);
    }
    
    private static string GetStateDataPath(StateMachine stateMachine)
    {
        string stateName = stateMachine.name;
        string stateMachineFolder = stateMachine.RootStateMachine.name;
        return string.Format("{0}/{1}/{2}RuntimeData.xml", DataFolder, stateMachineFolder, stateName);
    }
}

[XmlRoot("ActionDataCollection")]
public class ActionDataContainer
{
    public List<BaseAction> Actions = new List<BaseAction>();
}
