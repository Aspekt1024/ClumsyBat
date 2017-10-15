using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ifaces = DamageAction.Ifaces;

public class DamageNode : BaseNode {

    public int Damage = 1;

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
        AddOutput((int)Ifaces.Output);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
        SetInterface((int)Ifaces.Output, 1);
    }

    public override void Draw()
    {
        WindowRect.height = 80f;
        WindowRect.width = 130f;

        WindowTitle = "Take Damage";

        Damage = NodeGUI.IntFieldLayout("Amount:", Damage, 0.65f);
        NodeGUI.LabelLayout("* stops actions");

        SetInterfacePositions();
        DrawInterfaces();
        DrawConnections();
    }

    public override BaseAction GetAction()
    {
        return new DamageAction()
        {
            Damage = Damage
        };
    }
}
