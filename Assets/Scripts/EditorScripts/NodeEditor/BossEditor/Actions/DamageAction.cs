using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAction : BaseAction {

    public int Damage;

    private Boss bossScript;

    public enum Ifaces
    {
        Input
    }

    public override void ActivateBehaviour()
    {
        bossScript.TakeDamage(Damage);
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        bossScript = bossReference.GetComponent<Boss>();
    }
}
