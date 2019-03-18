using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = WalkAction.Ifaces;

public class WalkNode : BaseNode {
    
    public float walkSpeed;
    public float walkDuration;
    public WalkAction.WalkOptions walkOption;
    private int selectedWalkOptionIndex;

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
        AddInput((int)Ifaces.Direction, InterfaceTypes.Object);

        AddOutput((int)Ifaces.StartWalk);
        AddOutput((int)Ifaces.EndWalk);
        AddOutput((int)Ifaces.HitWall);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
        SetInterface((int)Ifaces.Direction, 6);

        SetInterface((int)Ifaces.StartWalk, 1, "Begin");
        SetInterface((int)Ifaces.EndWalk, 2, "End");
        SetInterface((int)Ifaces.HitWall, 3, "Hit Wall");
    }

    public override void Draw()
    {
        WindowTitle = "Walk";
        Transform.Width = 170;
        Transform.Height = 150;

        NodeGUI.Space(3);
        walkSpeed = NodeGUI.FloatFieldLayout(walkSpeed, "Speed:");
        walkDuration = NodeGUI.FloatFieldLayout(walkDuration, "Duration:");
        if (GetInterface((int)Ifaces.Direction).IsConnected())
            NodeGUI.LabelLayout("To " + GetInterface((int)Ifaces.Direction).ConnectedInterface.GetNode().WindowTitle);
        else
            walkOption = (WalkAction.WalkOptions)NodeGUI.EnumPopupLayout("Direction:", walkOption);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new WalkAction()
        {
            WalkDuration = walkDuration,
            WalkSpeed = walkSpeed,
            WalkOption = walkOption
        };
    }

    private void AddSpaces(int numSpaces)
    {
        for (int i = 0; i < numSpaces; i++)
        {
            EditorGUILayout.LabelField("");
        }
    }
}
