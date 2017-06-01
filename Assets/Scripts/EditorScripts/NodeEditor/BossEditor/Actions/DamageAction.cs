using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAction : BaseAction {

    public DamageTypes DamageType;

    public enum DamageTypes
    {
        Hypersonic, Dash, Player, FallingStalactite, StaticStalactite
    }

    public enum Ifaces
    {
        Output, Other
    }

    private DamageTypes receivedDamageType;
    private Collider2D other;
    
    public void SetReceivedDamageType(DamageTypes type)
    {
        receivedDamageType = type;
    }
    
    public void SetOther(Collider2D o)
    {
        other = o;
    }

    public override void ActivateBehaviour()
    {
        IsActive = false;

        if (receivedDamageType == DamageType)
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
