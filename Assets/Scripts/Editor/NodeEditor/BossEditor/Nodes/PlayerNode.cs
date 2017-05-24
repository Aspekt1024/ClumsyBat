using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class PlayerNode : BaseNode {

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
        Transform.Width = 80;
        Transform.Height = 40;
        WindowTitle = "Player";

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new PlayerAction();
    }
}
