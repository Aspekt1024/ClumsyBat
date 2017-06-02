using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Ifaces = CollisionAction.Ifaces;
using CollisionTypes = CollisionAction.CollisionTypes;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class CollisionNode : BaseNode {

    public CollisionTypes CollisionType;

    protected override void AddInterfaces()
    {
        AddOutput((int)Ifaces.Output);
        AddOutput((int)Ifaces.Other, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Output, 1);
        SetInterface((int)Ifaces.Other, 2, "Other");
    }

    public override void Draw()
    {
        Transform.Width = 200;
        Transform.Height = 70;
        WindowTitle = "Collision";

        CollisionType = (CollisionTypes)NodeGUI.EnumPopupLayout("Type:", CollisionType, 0.25f);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new CollisionAction()
        {
            CollisionType = CollisionType
        };
    }
}
