﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatingAction : BaseAction {

    public int LatchCount;
    public AlternatingMode Mode;
    
    public enum Ifaces
    {
        Input, Reset,
        Output1, Output2
    }

    public enum AlternatingMode
    {
        Normal, NoLoop
    }

    private bool firstOutput = true;
    private bool waitedOneTick = true;
    private int currentCount;

    protected override void ActivateBehaviour()
    {
        if (GetInterface((int)Ifaces.Reset).WasCalled())
        {
            GetInterface((int)Ifaces.Reset).UseCall();
            firstOutput = true;

            currentCount = 0;
        }

        if (GetInterface((int)Ifaces.Input).WasCalled())
        {
            GetInterface((int)Ifaces.Input).UseCall();
            IsActive = true;
            waitedOneTick = false;  // Allows reset to be called first
        }
    }

    public override void Tick(float deltaTime)
    {
        if (!IsActive) return;
        IsActive = false;

        if (waitedOneTick) return;
        waitedOneTick = true;
        
        if (firstOutput)
        {
            CallNext((int)Ifaces.Output1);
        }
        else
        {
            CallNext((int)Ifaces.Output2);
        }

        currentCount++;
        if (currentCount == LatchCount)
        {
            firstOutput = !firstOutput;
            if (Mode == AlternatingMode.Normal)
            {
                currentCount = 0;
            }
        }
    }

}
