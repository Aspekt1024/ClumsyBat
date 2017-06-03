using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ifaces = AlternatingAction.Ifaces;

public class AlternatingNode : BaseNode {

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
        AddOutput((int)Ifaces.Output1);
        AddOutput((int)Ifaces.Output2);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);
        SetInterface((int)Ifaces.Output1, 1);
        SetInterface((int)Ifaces.Output2, 2);
    }

    public override void Draw()
    {
        WindowTitle = "Alternate";
        WindowRect.size = new Vector2(80, 70);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new AlternatingAction();
    }
}
