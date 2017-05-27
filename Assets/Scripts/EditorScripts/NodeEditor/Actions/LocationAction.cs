using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationAction : BaseAction {

    public enum Ifaces
    {
        Input1, Input2,
        OutX, OutY, OutDist, OutDX, OutDY
    }

    public Vector2 Pos1 = Vector2.zero;
    public Vector2 Pos2 = Vector2.zero;

    private int Pos1ConnIndex = -1;
    private int Pos2ConnIndex = -1;

    public override void ActivateBehaviour() { }

    public override float GetFloat(int connID)
    {
        if (Pos1ConnIndex < 0 || Pos2ConnIndex < 0)
            GetPositionConnIDs();

        GetPositions();

        float output = 0;
        switch(connID)
        {
            case (int)Ifaces.OutX:
                output = Pos1.x;
                break;

            case (int)Ifaces.OutY:
                output = Pos1.y;
                break;

            case (int)Ifaces.OutDist:
                output = Vector2.Distance(Pos1, Pos2);
                break;

            case (int)Ifaces.OutDX:
                output = Mathf.Abs(Pos2.x - Pos1.x);
                break;

            case (int)Ifaces.OutDY:
                output = Mathf.Abs(Pos2.y - Pos1.y);
                break;
        }
        return output;
    }

    private void GetPositions()
    {
        ActionConnection input1 = connections[Pos1ConnIndex];
        ActionConnection input2 = connections[Pos2ConnIndex];

        if (input1.IsConnected())
        {
            Pos1 = input1.ConnectedInterface.Action.GetPosition(input1.OtherConnID);
        }

        if (input2.IsConnected())
        {
            Pos2 = input2.ConnectedInterface.Action.GetPosition(input2.OtherConnID);
        }
    }

    private void GetPositionConnIDs()
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].ID == (int)Ifaces.Input1)
                Pos1ConnIndex = i;
            else if (connections[i].ID == (int)Ifaces.Input2)
                Pos2ConnIndex = i;
        }
    }
}
