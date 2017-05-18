using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IODirection = NodeInterface.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class PlayerNode : BaseNode {

    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Output, 0, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInterface(25f, 0, "Object");
    }

    public override void Draw()
    {
        Transform.Width = 80;
        Transform.Height = 40;
        WindowTitle = "Player";

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        //Action = new PlayerAction();
    }
}
