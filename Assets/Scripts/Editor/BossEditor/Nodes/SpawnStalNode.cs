using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using StalActions = SpawnStalAction.StalActions;
using StalSpawnDirection = SpawnStalAction.StalSpawnDirection;
using Inputs = SpawnStalAction.Inputs;
using StalSpawnType = SpawnStalAction.StalSpawnType;

public class SpawnStalNode : BaseNode {

    public StalActions selectedStalAction;
    public int selectedStalActionIndex;

    public StalSpawnDirection SpawnDirection;
    public int selectedSpawnDirectionIndex;

    [SerializeField]
    private int numStals = 1;
    [SerializeField]
    private List<StalSpawnType> stalSpawns = new List<StalSpawnType>();

    protected override void AddInterfaces()
    {
        AddInput((int)Inputs.Main);

        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(30, (int)Inputs.Main);
        SetOutput(30);
    }

    public override void Draw()
    {
        WindowTitle = "Stalactites";
        Transform.Width = 180;
        Transform.Height = 102 + stalSpawns.Count * 19;
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUIUtility.labelWidth = 70;
        var stalActionArray = Enum.GetValues(typeof(StalActions));
        var stalActionStringArray = EditorHelpers.GetEnumStringArray(typeof(StalActions));
        selectedStalActionIndex = EditorGUILayout.Popup("Stal action:", selectedStalActionIndex, stalActionStringArray);
        selectedStalAction = (StalActions)stalActionArray.GetValue(selectedStalActionIndex);

        if (selectedStalAction != StalActions.Drop)
        {
            var stalDirectionArray = Enum.GetValues(typeof(StalSpawnDirection));
            var stalDirectionStringArray = EditorHelpers.GetEnumStringArray(typeof(StalSpawnDirection));
            selectedSpawnDirectionIndex = EditorGUILayout.Popup("Direction:", selectedSpawnDirectionIndex, stalDirectionStringArray);
            SpawnDirection = (StalSpawnDirection)stalDirectionArray.GetValue(selectedSpawnDirectionIndex);
            DisplayStalListForEditing();
        }
        else
        {
            Transform.Height = 80f;    // TODO this just hides the stal pos input interface set if it exists
        }

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<SpawnStalAction>();
        ((SpawnStalAction)Action).StalAction = selectedStalAction;
        ((SpawnStalAction)Action).SpawnDirection = SpawnDirection;
        ((SpawnStalAction)Action).stalSpawns = stalSpawns;
    }

    public override void SetupNode(BossDataContainer dataContainer)
    {
        base.SetupNode(dataContainer);
        AddNewStalSpawn();
    }

    private void DisplayStalListForEditing()
    {
        CheckForListCountChange();
        numStals = EditorGUILayout.IntField("Num Stals:", numStals);

        for (int i = 0; i < stalSpawns.Count; i++)
        {
            Vector2 startPos = new Vector2(15f, 95f + i * 19f);

            var spawn = stalSpawns[i];
            if (inputs[spawn.inputIndex].connectedNode != null)
            {
                EditorGUI.LabelField(new Rect(startPos.x, startPos.y, 150f, 15f), "At " + inputs[spawn.inputIndex].connectedNode.WindowTitle);
            }
            else
            {
                EditorGUI.LabelField(new Rect(startPos.x, startPos.y, 30f, 15f), "rng:");
                EditorGUI.MinMaxSlider(new Rect(startPos.x + 30, startPos.y, 65f, 15), ref spawn.xPosStart, ref spawn.xPosEnd, -6.2f, 6.2f);
                SetInput(startPos.y + 8f, spawn.inputIndex);
            }
            EditorGUI.LabelField(new Rect(startPos.x + 100f, startPos.y, 40f, 15f), "dly:");
            spawn.delay = EditorGUI.FloatField(new Rect(startPos.x + 125f, startPos.y, 30f, 15f), spawn.delay);
            stalSpawns[i] = spawn;
        }
    }

    private void AdjustStalSpawnListCount(int numStals)
    {
        while (stalSpawns.Count < numStals)
        {
            AddNewStalSpawn();
        }
        while (stalSpawns.Count > numStals)
        {
            int index = stalSpawns.Count - 1;
            RemoveStalSpawn(index);
        }
    }

    private void AddNewStalSpawn()
    {
        var newSpawn = new StalSpawnType()
        {
            inputIndex = inputs.Count
        };
        stalSpawns.Add(newSpawn);
        AddInput(inputs.Count, InterfaceTypes.Object);
    }

    private void RemoveStalSpawn(int index)
    {
        inputs.Remove(inputs[stalSpawns[index].inputIndex]);
        stalSpawns.Remove(stalSpawns[index]);
    }

    private void  CheckForListCountChange()
    {
        if (Event.current.type == EventType.keyDown && numStals != stalSpawns.Count)
        {
            if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
            {
                AdjustStalSpawnListCount(numStals);
            }
        }
    }

}
