using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IODirection = NodeInterface.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = ChargeAction.Ifaces;

public class ChargeNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInterface(IODirection.Input, (int)Ifaces.Input);
        AddInterface(IODirection.Output, (int)Ifaces.Charging);
        AddInterface(IODirection.Output, (int)Ifaces.HitWall);
        AddInterface(IODirection.Output, (int)Ifaces.Recovered);
    }

    private void SetInterfacePositions()
    {
        SetInterface(30f, (int)Ifaces.Input);
        SetInterface(30f, (int)Ifaces.Charging, "Charging");
        SetInterface(50f, (int)Ifaces.HitWall, "Hit Wall");
        SetInterface(70f, (int)Ifaces.Recovered, "Recovered");
    }

    public override void Draw()
    {
        Transform.Width = 110;
        Transform.Height = 82;
        WindowTitle = "Charge";

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = new ChargeAction();
    }

}
