using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ifaces = AlternatingAction.Ifaces;

public class AlternatingNode : BaseNode {
    
    public int LatchCount;

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
        AddInput((int)Ifaces.Reset);
        AddOutput((int)Ifaces.Output1);
        AddOutput((int)Ifaces.Output2);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
        SetInterface((int)Ifaces.Reset, 2, "reset");
        SetInterface((int)Ifaces.Output1, 1);
        SetInterface((int)Ifaces.Output2, 2);
    }

    public override void Draw()
    {
        WindowTitle = "Alternate";
        WindowRect.size = new Vector2(80, 70);

        if (LatchCount < 1) LatchCount = 1;
        LatchCount = NodeGUI.IntFieldLayout("i =", LatchCount);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new AlternatingAction()
        {
            LatchCount = LatchCount
        };
    }
}
