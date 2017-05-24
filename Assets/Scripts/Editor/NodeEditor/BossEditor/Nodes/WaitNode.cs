using UnityEngine;
using UnityEditor;
using System;

using Iface = WaitAction.Ifaces;

public class WaitNode : BaseNode {

    public float WaitTime = 1f;
    
    protected override void AddInterfaces()
    {
        AddInput((int)Iface.Input);
        AddOutput((int)Iface.Output);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Iface.Input, 1);
        SetInterface((int)Iface.Output, 1);
    }

    public override void Draw()
    {
        WindowTitle = "Wait";
        Transform.Width = 70;
        Transform.Height = 60;

        WaitTime = NodeGUI.FloatFieldWithPrefixLayout(WaitTime, " s", 0.7f);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new WaitAction()
        {
            WaitTime = WaitTime
        };
    }
}
