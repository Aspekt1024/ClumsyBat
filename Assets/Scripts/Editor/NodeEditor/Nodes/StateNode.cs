using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

using StateChangeTypes = BossState.StateChangeTypes;

/// <summary>
/// A node representing a state in the state machine.
/// It provides a window into the State by pairing the
/// MachineState Actions (runtime) and the NodeData (editor)
/// </summary>
public class StateNode : BaseNode {

    public string StateName = "New State";

    [XmlIgnore] public BossState State;

    private int selectedStateIndex;

    public override void SetupNode(StateMachine dataContainer)
    {
        base.SetupNode(dataContainer);
    }
    
    protected override void AddInterfaces()
    {
        AddInterface(ActionConnection.IODirection.Input, 0);
        AddInterface(ActionConnection.IODirection.Output, 1);
    }

    private void SetInterfacePositions()
    {
        SetInterface(0, 1);
        SetInterface(1, 1);
    }

    public override void Draw()
    {
        Transform.Width = 250;
        Transform.Height = 150;
        WindowTitle = State == null ? "New State" : State.StateName;

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
    
    private void DisplayStateInfo()
    {
        foreach (var stateEvent in State.StateEvents)
        {
            NodeGUI.LabelLayout(stateEvent.EventName);
        }
        //State.StateChange = (StateChangeTypes)NodeGUI.EnumPopupLayout("Change state on:", State.StateChange, 0.5f);
        //GetStateChangeData();

        // TODO absorb these as events in the State machine
        //State.DamagedByHypersonic = EditorGUILayout.Toggle("Damaged by Hypersonic?", State.DamagedByHypersonic);    // TODO dropdown with add button - should be a list. this is messy.
        //State.DamagedByStalactites = EditorGUILayout.Toggle("Damaged by Stalactites?", State.DamagedByStalactites);
        //State.DamagedByPlayer = EditorGUILayout.Toggle("Damaged by Player?", State.DamagedByPlayer);
    }

    private void GetStateChangeData()
    {
        switch (State.StateChange)
        {
            case StateChangeTypes.Health:
                State.MoveOnHP = EditorGUILayout.IntField("Move when HP =", State.MoveOnHP);
                break;
            case StateChangeTypes.NumLoops:
                State.MoveOnLoops = EditorGUILayout.IntField("Move on x loops:", State.MoveOnLoops);
                break;
            case StateChangeTypes.Time:
                State.MoveAfterSeconds = EditorGUILayout.FloatField("Move after seconds:", State.MoveAfterSeconds);
                break;
        }
    }

    private void DisplayStateSelect()
    {

        StateName = NodeGUI.TextFieldLayout(StateName, "State Name:");
        NodeGUI.Space(0.2f);
        if (NodeGUI.ButtonLayout("Create new State"))
        {
            CreateNewState();
        }

        NodeGUI.Space();
        BossState[] allStates = Resources.LoadAll<BossState>("NPCs/Bosses/BossBehaviours");
        if (allStates.Length == 0) return;

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
        BossState newState = ScriptableObject.CreateInstance<BossState>();
        newState.StateName = StateName;
        newState.RootStateMachine = ParentEditor.StateMachine.RootStateMachine;
        newState.BossName = newState.RootStateMachine.BossName;

        string dataFolder = NodeEditorSaveHandler.DataFolder;
        string bossFolder = newState.BossName.Replace(" ", "");
        NodeEditorSaveHandler.CreateFolderIfNotExists(dataFolder, bossFolder);
        string bossDataPath = string.Format("{0}/{1}", dataFolder, bossFolder);
        
        string stateFolder = StateName.Replace(" ", "");
        NodeEditorSaveHandler.CreateFolderIfNotExists(bossDataPath, stateFolder);

        string assetName = stateFolder + ".asset";
        string dataPath = string.Format("{0}/{1}/{2}", bossDataPath, stateFolder, assetName);
        
        AssetDatabase.CreateAsset(newState, dataPath);
        State = newState;
    }

    private void UseExistingState(BossState existingState)
    {
        State = existingState;
    }

    public override BaseAction GetAction()
    {
        return new StateAction()
        {
            StateName = StateName
        };
    }

}
