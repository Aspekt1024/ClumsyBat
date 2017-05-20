using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class SpawnMothNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, 0);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30, 0);
    }

    public override void Draw()
    {
        WindowTitle = "Spawn Moth";
        Transform.Width = 120;
        Transform.Height = 60;

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new SpawnMothAction();
    }
}
