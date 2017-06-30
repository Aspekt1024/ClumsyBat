
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = AirChargeAction.Ifaces;
using Directions = AirChargeAction.Directions;

public class AirChargeNode : BaseNode {

    public float ChargeSpeed;
    public Directions ChargeDirection;

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
        AddInput((int)Ifaces.Direction, InterfaceTypes.Object);

        AddOutput((int)Ifaces.Charging);
        AddOutput((int)Ifaces.HitWall);
        AddOutput((int)Ifaces.Recovered);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);

        string directionLabel = "Dir:";
        if (GetInterface((int)Ifaces.Direction).IsConnected())
            directionLabel = "At " + GetInterface((int)Ifaces.Direction).ConnectedInterface.GetNode().WindowTitle;
        SetInterface((int)Ifaces.Direction, 5, directionLabel);

        SetInterface((int)Ifaces.Charging, 1, "Charging");
        SetInterface((int)Ifaces.HitWall, 2, "Hit Wall");
        SetInterface((int)Ifaces.Recovered, 3, "Recovered");
    }

    public override void Draw()
    {
        WindowTitle = "Air Charge";
        WindowRect.size = new Vector2(140f, 130f);

        NodeGUI.Space(2.85f);
        ChargeSpeed = NodeGUI.FloatFieldLayout(ChargeSpeed, "Speed:");
        NodeGUI.Space(.15f);

        if (GetInterface((int)Ifaces.Direction).IsConnected())
        {
            ChargeDirection = Directions.Other;
        }
        else
        {
            ChargeDirection = (Directions)NodeGUI.EnumPopupLayout("", ChargeDirection, 0.3f);
            if (ChargeDirection == Directions.Other)
                ChargeDirection = Directions.Left;  // TODO make player not show up...
        }

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new AirChargeAction()
        {
            ChargeSpeed = ChargeSpeed,
            ChargeDirection = ChargeDirection
        };
    }

}
