
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IODirection = ActionConnection.IODirection;
using InterfaceTypes = NodeInterface.InterfaceTypes;

using Ifaces = AirFollowAction.Ifaces;

public class AirFollowNode : BaseNode {

    public float FollowSpeed;

    protected override void AddInterfaces()
    {
        AddInput((int)Ifaces.Input);
        AddInput((int)Ifaces.Direction, InterfaceTypes.Object);

        AddOutput((int)Ifaces.Next);
    }

    private void SetInterfacePositions()
    {
        SetInterface((int)Ifaces.Input, 1);

        string directionLabel = "Dir:";
        if (GetInterface((int)Ifaces.Direction).IsConnected())
            directionLabel = "At " + GetInterface((int)Ifaces.Direction).ConnectedInterface.GetNode().WindowTitle;
        SetInterface((int)Ifaces.Direction, 4, directionLabel);

        SetInterface((int)Ifaces.Next, 1, "Next");
    }

    public override void Draw()
    {
        WindowTitle = "Follow (Flight)";
        WindowRect.size = new Vector2(140f, 110f);

        NodeGUI.Space();
        FollowSpeed = NodeGUI.FloatFieldLayout(FollowSpeed, "Speed:");
        
        SetInterfacePositions();
        DrawInterfaces();
    }

    public override BaseAction GetAction()
    {
        return new AirFollowAction()
        {
            FollowSpeed = FollowSpeed
        };
    }

}
