using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatingAction : BaseAction {

    public int LatchCount;
    
    public enum Ifaces
    {
        Input, Reset,
        Output1, Output2
    }

    private bool firstOutput = true;
    private int currentCount;

    protected override void ActivateBehaviour()
    {
        IsActive = false;
        currentCount++;
        
        if (GetInterface((int)Ifaces.Reset).WasCalled())
        {
            GetInterface((int)Ifaces.Reset).UseCall();
            firstOutput = true;
            currentCount = 0;
            return;
        }
        
        if (firstOutput)
        {
            CallNext((int)Ifaces.Output1);
        }
        else
        {
            CallNext((int)Ifaces.Output2);
        }

        if (currentCount == LatchCount)
        {
            firstOutput = !firstOutput;
            currentCount = 0;
        }
    }

}
