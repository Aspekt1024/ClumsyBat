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
    
    [SerializeField]
    private int selectedStateChangeIndex;
    
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
        SetInterface(8f, 0);
        SetInterface(8f, 1);
    }

    public override void Draw()
    {
        Transform.Width = 200;
        Transform.Height = 120;
        WindowTitle = State == null ? "New State" : State.StateName;

        if (State != null)
            DisplayStateInfo();
        else
            DisplayStateSelect();

        SetInterfacePositions();
        DrawInterfaces();

    }
    
    private void DisplayStateInfo()
    {
        EditorGUIUtility.labelWidth = 115;
        State.StateChange = (StateChangeTypes)EditorGUILayout.EnumPopup("Change state on:", State.StateChange);
        GetStateChangeData();

        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 160;
        State.DamagedByHypersonic = EditorGUILayout.Toggle("Damaged by Hypersonic?", State.DamagedByHypersonic);    // TODO dropdown with add button - should be a list. this is messy.
        State.DamagedByStalactites = EditorGUILayout.Toggle("Damaged by Stalactites?", State.DamagedByStalactites);
        State.DamagedByPlayer = EditorGUILayout.Toggle("Damaged by Player?", State.DamagedByPlayer);
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
        BossState[] allStates = Resources.LoadAll<BossState>("NPCs/Bosses/BossScriptableObjects");
        if (allStates.Length > 0)
        {
            var selectedStateIndex = BossSelectorHelpers.GetIndexFromObject(allStates, State);
            if (selectedStateIndex < 0) selectedStateIndex = 0;

            var statesStringArray = BossSelectorHelpers.ObjectArrayToStringArray(allStates);
                
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.Popup("Existing:", selectedStateIndex, statesStringArray);
            if (GUILayout.Button("Use existing state"))
            {
                UseExistingState(allStates[selectedStateIndex]);
            }

            EditorGUILayout.Space();
        }
        else
        {
            EditorGUILayout.LabelField("No existing states found.");
            EditorGUILayout.Space();
        }

        EditorGUIUtility.labelWidth = 80;

        StateName = NodeGUI.TextFieldLayout(StateName, "State Name:");
        StateName = NodeGUI.TextFieldLayout(StateName, "State Name:");

        if (NodeGUI.Button(new Rect(1, 7, 18, 2), "Create new State"))
        {
            CreateNewState();
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
