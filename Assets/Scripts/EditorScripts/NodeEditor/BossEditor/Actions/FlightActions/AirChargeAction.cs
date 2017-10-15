using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirChargeAction : BaseAction {

    public float ChargeSpeed;
    public Directions ChargeDirection;

    public enum Ifaces
    {
        Input, Direction,
        Charging, HitWall, Recovered
    }

    public enum Directions
    {
        Left, Right, Other
    }

    private AirChargeAbility charge;

    public float GetInputXPosition()
    {
        ActionConnection conn = GetInterface((int)Ifaces.Direction);
        Vector2 pos = conn.ConnectedInterface.Action.GetPosition(conn.OtherConnID);
        return pos.x;
    }

    protected override void ActivateBehaviour()
    {
        charge.Activate(this);
        CallNext((int)Ifaces.Charging);
    }

    public void HitWall()
    {
        CallNext((int)Ifaces.HitWall);
    }

    public void Recovered()
    {
        IsActive = false;
        CallNext((int)Ifaces.Recovered);
    }

    public override void GameSetup(BehaviourSet owningContainer, BossData behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        charge = bossData.GetAbility<AirChargeAbility>();
    }

    public override void Stop()
    {
        IsStopped = true;
        if (!IsActive) return;

        IsActive = false;
        charge.Interrupt();
    }
}
