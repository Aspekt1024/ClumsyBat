using System;
using System.Xml.Serialization;
using UnityEngine;

using Ifaces = PositionAction.Ifaces;
using InterfaceTypes = NodeInterface.InterfaceTypes;

public class PositionNode : BaseNode {

    public Vector2 Pos1;
    public Vector2 Pos2;

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input1, InterfaceTypes.Object);
        AddInput((int)Ifaces.Input2, InterfaceTypes.Object);

        AddOutput((int)Ifaces.OutDist, InterfaceTypes.Object);
        AddOutput((int)Ifaces.OutDX, InterfaceTypes.Object);
        AddOutput((int)Ifaces.OutDY, InterfaceTypes.Object);
        AddOutput((int)Ifaces.OutX, InterfaceTypes.Object);
        AddOutput((int)Ifaces.OutY, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input1, 1, "In1");
        SetInterface((int)Ifaces.Input2, 2, "In2");

        SetInterface((int)Ifaces.OutDist, 1, "dist");
        SetInterface((int)Ifaces.OutDX, 2, "dX");
        SetInterface((int)Ifaces.OutDY, 3, "dY");

        HideInterface((int)Ifaces.OutX);
        HideInterface((int)Ifaces.OutY);
    }

    public override void Draw()
    {
        Transform.Width = 100;
        Transform.Height = 100;
        WindowTitle = "Position";
        

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new PositionAction()
        {
            Pos1 = Pos1,
            Pos2 = Pos2
        };
    }
}
