using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class RestartMachineNode : BaseNode {

    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, 0);
    }

    private void SetInterfacePositions()
    {
        SetInterface(0, 1);
    }

    public override void Draw()
    {
        WindowTitle = "Restart Machine";
        Transform.Width = 120;
        Transform.Height = 50;
        nodeType = NodeTypes.Event;

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new RestartMachineAction();
    }
}
