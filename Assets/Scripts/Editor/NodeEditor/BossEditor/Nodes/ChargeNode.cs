
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IODirection = ActionConnection.IODirection;
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
        SetInterface((int)Ifaces.Input, 1);

        SetInterface((int)Ifaces.Charging, 1, "Charging");
        SetInterface((int)Ifaces.HitWall, 2, "Hit Wall");
        SetInterface((int)Ifaces.Recovered, 3, "Recovered");
    }

    public override void Draw()
    {
        Transform.Width = 110;
        Transform.Height = 82;
        WindowTitle = "Charge";

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new ChargeAction();
    }

}
