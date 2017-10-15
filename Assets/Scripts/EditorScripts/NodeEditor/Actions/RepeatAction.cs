using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatAction : BaseAction {

    public int RepeatCount;
    public float DelayDuration;

    public enum Ifaces
    {
        Input,
        Action, Complete
    }
    
    private int currentCount;
    private float timer;

    protected override void ActivateBehaviour()
    {
        // First action is instantaneous
        CallNext((int)Ifaces.Action);
        currentCount = 1;
        timer = 0f;
    }

    public override void Tick(float deltaTime)
    {
        if (!IsActive) return;

        if (!Toolbox.Instance.GamePaused)
        {
            timer += deltaTime;
            if (timer >= DelayDuration)
            {
                CallNext((int)Ifaces.Action);
                currentCount++;
                timer = 0f;
            }
            
            if (currentCount == RepeatCount)
            {
                // Call complete without delay
                IsActive = false;
                CallNext((int)Ifaces.Complete);
            }
        }

    }
}
