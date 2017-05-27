using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

using StateChangeTypes = State.StateChangeTypes;

/// <summary>
/// A node representing a state in the state machine.
/// It provides a window into the State by pairing the
/// MachineState Actions (runtime) and the NodeData (editor)
/// </summary>
public class StateNode : BaseNode {

    public string StateName = "New State";

    [XmlIgnore] public State State;

    private int selectedStateIndex;

    public override void SetupNode(BehaviourSet dataContainer)
    {
        base.SetupNode(dataContainer);
    }
    
    protected override void AddInterfaces()
    {
        AddInput(0);
    }

    private void SetInterfacePositions()
    {
        SetInterface(0, 1);
    }

    public override void Draw()
    {
        WindowTitle = State == null ? "New State" : StateName;

        if (State != null)
        {
            DisplayStateInfo();
            SetInterfacePositions();
            DrawInterfaces();
        }
        else
        {
            DisplayStateSelect();
        }
    }

    public void AddNewStateEvent(int id)
    {
        AddOutput(id);  // TODO: id will be > 0. If we're adding a new interface to the state node, we must change this as id could be 1
    }

    public void RemoveStateEvent(int id)
    {
        interfaces.Remove(interfaces[id]);
    }
    
    private void DisplayStateInfo()
    {
        Transform.Width = 150f;
        Transform.Height = Mathf.Max(30f + 20f * (interfaces.Count - 1), 50f);

        foreach (var stateEvent in State.StateEvents)
        {
            for (int i = 0; i < interfaces.Count; i++)
            {
                if (stateEvent.ID == interfaces[i].ID)
                {
                    SetInterface(interfaces[i].ID, i);
                    interfaces[i].Label = stateEvent.EventName;
                    break;
                }
            } 
        }
    }

    private void DisplayStateSelect()
    {
        Transform.Width = 250;
        Transform.Height = 80;

        StateName = NodeGUI.TextFieldLayout(StateName, "State Name:");
        NodeGUI.Space(0.2f);
        if (NodeGUI.ButtonLayout("Create new State"))
        {
            CreateNewState();
        }

        NodeGUI.Space();
        State[] allStates = Resources.LoadAll<State>("NPCs/Bosses/BossBehaviours");
        if (allStates.Length == 0) return;

        Transform.Height = 150;
        var statesStringArray = BossSelectorHelpers.ObjectArrayToStringArray(allStates);
        selectedStateIndex = NodeGUI.PopupLayout("Existing State:", selectedStateIndex, statesStringArray);

        NodeGUI.Space(0.2f);
        if (NodeGUI.ButtonLayout("Use existing State"))
        {
            UseExistingState(allStates[selectedStateIndex]);
        }
    }

    private void CreateNewState()
    {
        State newState = ScriptableObject.CreateInstance<State>();
        newState.name = StateName;
        newState.ParentMachine = ParentEditor.BehaviourSet.ParentMachine;

        string dataFolder = BossActionLoadHandler.DataFolder;
        string bossFolder = newState.ParentMachine.name;
        NodeEditorSaveHandler.CreateFolderIfNotExists(dataFolder, bossFolder);
        string bossDataPath = string.Format("{0}/{1}", dataFolder, bossFolder);
        
        string stateFolder = StateName;
        NodeEditorSaveHandler.CreateFolderIfNotExists(bossDataPath, stateFolder);

        string assetName = stateFolder + ".asset";
        string dataPath = string.Format("{0}/{1}/{2}", bossDataPath, stateFolder, assetName);
        
        AssetDatabase.CreateAsset(newState, dataPath);
        State = newState;
    }

    private void UseExistingState(State existingState)
    {
        // TODO copy states
        Debug.Log("TODO copying states");
        return;



        State = existingState;
        List<BaseNode> stateNodes = NodeEditorSaveHandler.LoadStateMachineNodes(State.ParentMachine);

        foreach (var node in stateNodes)
        {
            if (node.IsType<StateNode>() && ((StateNode)node).StateName == State.name)
            {
                StateName = ((StateNode)node).StateName;
                foreach (var iface in node.interfaces.Where(iface => iface.Direction == ActionConnection.IODirection.Output))
                {
                    AddOutput(iface.ID);
                }
                break;
            }
        }
    }

    public override BaseAction GetAction()
    {
        return new StateAction()
        {
            StateName = StateName
        };
    }

}
