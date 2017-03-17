using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using StalActions = SpawnStalAction.StalActions;
using StalSpawnDirection = SpawnStalAction.StalSpawnDirection;
using Inputs = SpawnStalAction.Inputs;

public class SpawnStalNode : BaseNode {

    public StalActions selectedStalAction;
    public int selectedStalActionIndex = 1;

    public StalSpawnDirection SpawnDirection;
    public int selectedSpawnDirectionIndex = 1;


    protected override void AddInterfaces()
    {
        AddInput((int)Inputs.Main);
        AddInput((int)Inputs.PositionObj, InterfaceTypes.Object);

        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(25, (int)Inputs.Main);
        SetInput(65, (int)Inputs.PositionObj, "xPos");
        SetOutput(25);
    }

    public override void DrawWindow()
    {
        WindowTitle = "Spawn Stal";
        WindowRect.width = 150;
        WindowRect.height = 80;

        var stalActionArray = Enum.GetValues(typeof(StalActions));
        var stalActionStringArray = EditorHelpers.GetEnumStringArray(typeof(StalActions));
        var stalDirectionArray = Enum.GetValues(typeof(StalSpawnDirection));
        var stalDirectionStringArray = EditorHelpers.GetEnumStringArray(typeof(StalSpawnDirection));

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 70;
        selectedStalActionIndex = EditorGUILayout.Popup("Stal action:", selectedStalActionIndex, stalActionStringArray);
        selectedStalAction = (StalActions)stalActionArray.GetValue(selectedStalActionIndex);
        selectedSpawnDirectionIndex = EditorGUILayout.Popup("Direction:", selectedSpawnDirectionIndex, stalDirectionStringArray);
        SpawnDirection = (StalSpawnDirection)stalDirectionArray.GetValue(selectedSpawnDirectionIndex);

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<SpawnStalAction>();
        ((SpawnStalAction)Action).StalAction = selectedStalAction;
        ((SpawnStalAction)Action).SpawnDirection = SpawnDirection;
    }
    

}
