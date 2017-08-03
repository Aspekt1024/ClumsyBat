using UnityEngine;
using UnityEditor;
using System;

using Iface = TimerAction.Ifaces;

public class TimerNode : BaseNode {

    public float StartingValue;

    protected override void AddInterfaces()
    {
        AddInput((int)Iface.Start);
        AddInput((int)Iface.Reset);

        AddOutput((int)Iface.Value, NodeInterface.InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Iface.Start, 1, "start");
        SetInterface((int)Iface.Reset, 3, "reset");

        SetInterface((int)Iface.Value, 1, "timer");
    }

    public override void Draw()
    {
        WindowTitle = "Timer";
        Transform.Width = 100;
        Transform.Height = 90;

        NodeGUI.Space();
        StartingValue = NodeGUI.FloatFieldLayout(StartingValue, "Init: ");

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new TimerAction()
        {
            StartingValue = StartingValue
        };
    }
}
