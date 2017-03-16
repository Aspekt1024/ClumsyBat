using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using StalActions = SpawnStalAction.StalActions;
using Inputs = SpawnStalAction.Inputs;

public class SpawnStalNode : BaseNode {

    public StalActions selectedStalAction;
    public int selectedStalActionIndex = 1;

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

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 70;
        selectedStalActionIndex = EditorGUILayout.Popup("Stal action:", selectedStalActionIndex, stalActionStringArray);
        selectedStalAction = (StalActions)stalActionArray.GetValue(selectedStalActionIndex);

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<SpawnStalAction>();
        ((SpawnStalAction)Action).StalAction = selectedStalAction;
    }
    

}
