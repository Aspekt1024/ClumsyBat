using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAction : BaseAction {

    public enum Ifaces
    {
        Input, Charging, HitWall, Recovered
    }

    private ChargeAbility charge;

    public override void ActivateBehaviour()
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
        CallNext((int)Ifaces.Recovered);
    }

    public override void GameSetup(StateMachine owningContainer, BossData behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        charge = bossData.GetAbility<ChargeAbility>();
    }
}
