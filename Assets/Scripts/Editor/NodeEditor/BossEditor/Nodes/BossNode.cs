﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class BossNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Output, 0, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInterface(0, 1, "Object");
    }

    public override void Draw()
    {
        WindowTitle = "Boss";
        WindowRect.size = new Vector2(80f, 50f);
        nodeType = NodeTypes.Object;

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new BossAction();
    }
}
