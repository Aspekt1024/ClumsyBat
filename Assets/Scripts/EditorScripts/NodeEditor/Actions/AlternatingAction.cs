using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatingAction : BaseAction {

    public enum Ifaces
    {
        Input,
        Output1, Output2
    }

    private bool firstOutput;

    public override void ActivateBehaviour()
    {
        IsActive = false;

        firstOutput = !firstOutput;
        if (firstOutput)
            CallNext((int)Ifaces.Output1);
        else
            CallNext((int)Ifaces.Output2);
    }

}
