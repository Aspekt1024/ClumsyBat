using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAction : BaseAction {

    public enum Ifaces
    {
        Output
    }

    public float MinValue;
    public float MaxValue;

    public override void ActivateBehaviour()
    {
        // Do nothing
    }

    public override float GetFloat(int id)
    {
        if (MinValue > MaxValue)
        {
            MaxValue += MinValue;
            MinValue = MaxValue - MinValue;
            MaxValue -= MinValue;
        }
        return Random.Range(MinValue, MaxValue);
    }

}
