using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAction : BaseAction {

    public CollisionTypes CollisionType;

    public enum CollisionTypes
    {
        Hypersonic, Dash, Player, FallingStalactite, StaticStalactite
    }

    public enum Ifaces
    {
        Output, Other
    }

    private CollisionTypes receivedCollisionType;
    private Collider2D other;
    
    public void SetReceivedDamageType(CollisionTypes type)
    {
        receivedCollisionType = type;
    }
    
    public void SetOther(Collider2D o)
    {
        other = o;
    }

    protected override void ActivateBehaviour()
    {
        IsActive = false;

        if (receivedCollisionType == CollisionType)
            CallNext((int)Ifaces.Output);
        else
            IsNewActivation = false;
    }

    public override GameObject GetObject(int id)
    {
        return other.gameObject;
    }

    public override Vector2 GetPosition(int id)
    {
        return other.transform.position;
    }
}
