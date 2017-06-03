using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ifaces = RandomAction.Ifaces;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class RandomNode : BaseNode {

    public float MinValue;
    public float MaxValue;

    protected override void AddInterfaces()
    {
        AddOutput((int)Ifaces.Output, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Output, 1);
    }

    public override void Draw()
    {
        WindowTitle = "Random";
        WindowRect.size = new Vector2(100, 80);

        MinValue = NodeGUI.FloatFieldLayout(MinValue, "Min:");
        MaxValue = NodeGUI.FloatFieldLayout(MaxValue, "Max:");

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new RandomAction()
        {
            MinValue = MinValue,
            MaxValue = MaxValue
        };
    }
}
