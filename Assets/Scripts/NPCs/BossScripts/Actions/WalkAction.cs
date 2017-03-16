using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAction : BaseAction {

    private Walk walkAbility;

    public enum Outputs
    {
        StartWalk, EndWalk
    }

    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        walkAbility = bossBehaviour.GetAbility<Walk>();
    }

    public override void ActivateBehaviour()
    {
        walkAbility.Activate(this);
    }

    public void EndWalk()
    {
        CallNext((int)Outputs.EndWalk);
    }
}
