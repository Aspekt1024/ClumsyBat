﻿using UnityEngine;

public class WaitAction : BaseAction {

    public float WaitTime = 1f;

    private bool bWaitActive;
    private float timeWaited;

    public override void ActivateBehaviour()
    {
        bWaitActive = true;
        timeWaited = 0f;
    }

    public override void Tick(float deltaTime)
    {
        if (!bWaitActive) return;
        timeWaited += deltaTime;
        if (timeWaited > WaitTime)
        {
            bWaitActive = false;
            CallNext();
        }
    }

}
