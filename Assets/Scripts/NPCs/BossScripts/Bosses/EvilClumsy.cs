using System;
using UnityEngine;

public class EvilClumsy : Boss
{
    protected override void HealthUpdate()
    {
        Debug.Log("Evil clumsy goes 'Ow!'");
    }

    protected override Rigidbody2D GetRigidBody()
    {
        return GetComponent<Rigidbody2D>();
    }
}
