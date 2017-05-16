using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using IODirection = NodeInterface.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = WalkAction.Ifaces;

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
        AddInterface(IODirection.Input, (int)Ifaces.Input);

        AddInterface(IODirection.Output, (int)Ifaces.StartWalk);
        AddInterface(IODirection.Output, (int)Ifaces.EndWalk);
        AddInterface(IODirection.Output, (int)Ifaces.HitWall);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30, (int)Ifaces.Input);

        SetInterface(30, (int)Ifaces.StartWalk, "Begin");
        SetInterface(50, (int)Ifaces.EndWalk, "End");
        SetInterface(70, (int)Ifaces.HitWall, "Hit Wall");
    }

    public override void Draw()
    {
        WindowTitle = "Movement";
        Transform.Width = 170;
        Transform.Height = 145;

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
