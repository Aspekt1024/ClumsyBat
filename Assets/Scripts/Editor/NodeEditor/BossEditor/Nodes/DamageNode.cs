using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Ifaces = DamageAction.Ifaces;
using DamageTypes = DamageAction.DamageTypes;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class DamageNode : BaseNode {

    public DamageTypes DamageType;

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
        WindowTitle = "Damaged";

        DamageType = (DamageTypes)NodeGUI.EnumPopupLayout("Type:", DamageType, 0.25f);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new DamageAction()
        {
            DamageType = DamageType
        };
    }
}
