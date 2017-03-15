using System;
using UnityEngine;

public class EvilClumsy : Boss
{
    protected override void HealthUpdate()
    {
        Debug.Log("Evil clumsy goes 'Ow!'");
    }
}
