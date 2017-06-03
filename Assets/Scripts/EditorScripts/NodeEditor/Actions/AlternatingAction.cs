using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatingAction : BaseAction {

    public enum Ifaces
    {
        Input, Reset,
        Output1, Output2
    }

    private bool firstOutput;

    public override void ActivateBehaviour()
    {
        IsActive = false;

        if (GetInterface((int)Ifaces.Reset).WasCalled())
        {
            GetInterface((int)Ifaces.Reset).UseCall();
            firstOutput = false;
            return;
        }

        firstOutput = !firstOutput;
        if (firstOutput)
            CallNext((int)Ifaces.Output1);
        else
            CallNext((int)Ifaces.Output2);
    }

}
