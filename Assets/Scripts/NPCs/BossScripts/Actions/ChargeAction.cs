using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAction : BaseAction {

    public enum Outputs
    {
        Charging, HitWall, Recovered
    }

    private ChargeAbility charge;

    public override void ActivateBehaviour()
    {
        charge.Activate(this);
        CallNext((int)Outputs.Charging);
    }

    public void HitWall()
    {
        CallNext((int)Outputs.HitWall);
    }

    public void Recovered()
    {
        CallNext((int)Outputs.Recovered);
    }

    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        charge = bossBehaviour.GetAbility<ChargeAbility>();
    }
}
