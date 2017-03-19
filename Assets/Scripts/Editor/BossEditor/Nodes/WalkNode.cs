using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Outputs = WalkAction.Outputs;

public class WalkNode : BaseNode {

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float walkDuration;
    [SerializeField]
    private WalkAction.WalkOptions walkOption;
    private int selectedWalkOptionIndex;

    protected override void AddInterfaces()
    {
        AddInput();

        AddOutput((int)Outputs.StartWalk);
        AddOutput((int)Outputs.EndWalk);
        AddOutput((int)Outputs.HitWall);
    }

    private void SetInterfacePositions()
    {
        SetInput(30);
        SetOutput(30, (int)Outputs.StartWalk, "Begin");
        SetOutput(50, (int)Outputs.EndWalk, "End");
        SetOutput(70, (int)Outputs.HitWall, "Hit Wall");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Movement";
        WindowRect.width = 170;
        WindowRect.height = 145;

        AddSpaces(3);
        EditorGUILayout.Separator();

        EditorGUIUtility.labelWidth = 82f;
        var walkOptionArray = Enum.GetValues(typeof(WalkAction.WalkOptions));
        var walkOptionStringArray = EditorHelpers.GetEnumStringArray(typeof(WalkAction.WalkOptions));
        selectedWalkOptionIndex = EditorGUILayout.Popup("Walk option:", selectedWalkOptionIndex, walkOptionStringArray);
        walkOption = (WalkAction.WalkOptions)walkOptionArray.GetValue(selectedWalkOptionIndex);

        walkSpeed = EditorGUILayout.FloatField("Speed:", walkSpeed);
        walkDuration = EditorGUILayout.FloatField("Duration:", walkDuration);

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<WalkAction>();
        ((WalkAction)Action).WalkDuration = walkDuration;
        ((WalkAction)Action).WalkSpeed = walkSpeed;
        ((WalkAction)Action).WalkOption = walkOption;
    }

    private void AddSpaces(int numSpaces)
    {
        for (int i = 0; i < numSpaces; i++)
        {
            EditorGUILayout.LabelField("");
        }
    }
}
