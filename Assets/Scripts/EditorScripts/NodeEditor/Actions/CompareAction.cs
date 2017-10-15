using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareAction : BaseAction {

    public enum Ifaces
    {
        Input,
        In1, In2, In3,
        OutputTrue, OutputFalse
    }

    public enum Operations
    {
        Greater, Lesser, Equal, InRange, NotInRange
    }

    public Operations OperationType;

    public float Input1 = 0f;
    public float Input2 = 0f;   // Input 2 and 3 used for range operation
    public float Input3 = 0f;   // 2 is low, 3 is high

    private int In1ConnIndex = -1;
    private int In2ConnIndex = -1;
    private int In3ConnIndex = -1;

    private bool waitedOneTick;

    protected override void ActivateBehaviour()
    {
        waitedOneTick = false;
    }

    public override void Tick(float deltaTime)
    {
        if (!behaviourSet.IsEnabled || !IsActive) return;
        if (!waitedOneTick) // prevents infinite loops
        {
            waitedOneTick = true;
            return;
        }

        if (In1ConnIndex < 0 || In2ConnIndex < 0 || In3ConnIndex < 0)
            GetInputConnIDs();

        GetInputs();

        bool result = false;
        switch (OperationType)
        {
            case Operations.Lesser:
                result = Input1 < Input2;
                break;
            case Operations.Greater:
                result = Input1 > Input2;
                break;
            case Operations.Equal:
                result = (int)Input1 == (int)Input2;
                break;
            case Operations.InRange:
                result = Input2 <= Input1 && Input1 <= Input3;
                break;
            case Operations.NotInRange:
                result = Input2 >= Input1 && Input1 >= Input3;
                break;
        }

        IsActive = false;

        if (result)
            CallNext((int)Ifaces.OutputTrue);
        else
            CallNext((int)Ifaces.OutputFalse);
    }

    public void GetInputs()
    {
        ActionConnection conn1 = connections[In1ConnIndex];
        ActionConnection conn2 = connections[In2ConnIndex];
        ActionConnection conn3 = connections[In3ConnIndex];

        if (conn1.IsConnected()) Input1 = conn1.ConnectedInterface.Action.GetFloat(conn1.OtherConnID);
        if (conn2.IsConnected()) Input2 = conn2.ConnectedInterface.Action.GetFloat(conn2.OtherConnID);
        if (conn3.IsConnected()) Input3 = conn3.ConnectedInterface.Action.GetFloat(conn3.OtherConnID);
    }

    private void GetInputConnIDs()
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].ID == (int)Ifaces.In1)
                In1ConnIndex = i;
            else if (connections[i].ID == (int)Ifaces.In2)
                In2ConnIndex = i;
            else if (connections[i].ID == (int)Ifaces.In3)
                In3ConnIndex = i;
        }
    }
}
