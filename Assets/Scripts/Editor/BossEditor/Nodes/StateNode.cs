using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using StateChangeTypes = BossState.StateChangeTypes;

/// <summary>
/// A node representing a state in the state machine.
/// It provides a window into the State by pairing the
/// State (runtime) and its NodeData (editor)
/// </summary>
public class StateNode : BaseNode {

    public BossState State;
    public BossEditorNodeData NodeData;

    private string newStateName = "New State";
    private int selectedStateIndex;

    [SerializeField]
    private int selectedStateChangeIndex;
    
    public override void SetupNode(BossDataContainer dataContainer)
    {
        base.SetupNode(dataContainer);
    }
    
    protected override void AddInterfaces()
    {
        AddInput();
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(8f);
        SetOutput(8f);
    }

    public override void DrawWindow()
    {
        WindowRect.width = 200;
        WindowRect.height = 120;
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
        // TODO use EditorHelpers.GetEnumStringArray(typeof(StateChangeTypes));
        var ChoiceArray = Enum.GetValues(typeof(StateChangeTypes));
        string[] choiceStringArray = new string[ChoiceArray.Length];
        for (int i = 0; i < ChoiceArray.Length; i++)
        {
            choiceStringArray[i] = ChoiceArray.GetValue(i).ToString();
        }
        EditorGUIUtility.labelWidth = 115;
        selectedStateChangeIndex = EditorGUILayout.Popup("Change state on:", selectedStateChangeIndex, choiceStringArray);
        State.StateChange = (StateChangeTypes)ChoiceArray.GetValue(selectedStateChangeIndex);
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
        newStateName = EditorGUILayout.TextField("State Name:", newStateName);
        if (GUILayout.Button("Create new state"))
        {
            CreateNewState();
        }
    }

    private void CreateNewState()
    {
        BossState newState = CreateInstance<BossState>();
        newState.StateName = newStateName;
        newState.BossName = DataContainer.BossName;
        newState.RootContainer = DataContainer;

        string dataFolder = EditorHelpers.GetAssetDataFolder(DataContainer);
        string subFolder = "States";
        EditorHelpers.CreateFolderIfNotExist(dataFolder, subFolder);
        string assetName = newStateName.Replace(" ", "") + ".asset";
        string dataPath = string.Format("{0}/{1}/{2}", dataFolder, subFolder, assetName);

        AssetDatabase.CreateAsset(newState, dataPath);
        State = newState;

        NodeData = CreateInstance<BossEditorNodeData>();
        dataFolder = EditorHelpers.GetDataPath(DataContainer);
        EditorHelpers.CreateFolderIfNotExist(dataFolder, "States");
        dataPath = string.Format("{0}/States/{1}", dataFolder, assetName);
        AssetDatabase.CreateAsset(NodeData, dataPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void UseExistingState(BossState existingState)
    {
        State = existingState;
        string dataFolder = EditorHelpers.GetDataPath(DataContainer) + "/States";
        string assetName = existingState.StateName.Replace(" ", "") + ".asset";
        string dataPath = string.Format("{0}/{1}", dataFolder, assetName);
        NodeData = AssetDatabase.LoadAssetAtPath<BossEditorNodeData>(dataPath);
    }
    
    protected override void CreateAction()
    {
        MachineState machineState = CreateInstance<MachineState>();
        if (NodeData == null) return;

        StartNode start = NodeData.GetStartNode();
        if (start != null)
        {
            machineState.State = State;
        }

        Action = machineState;
    }

}
