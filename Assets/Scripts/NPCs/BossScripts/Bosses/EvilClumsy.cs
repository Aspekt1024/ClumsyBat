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

    protected override void GetBossComponents()
    {
        Body = GetRigidBody();
        bossCollider = GetComponentInChildren<Collider2D>();
        bossRenderer = GetComponent<SpriteRenderer>();
    }
}
