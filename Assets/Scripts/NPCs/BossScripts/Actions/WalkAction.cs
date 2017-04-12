using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAction : BaseAction {

    public enum WalkOptions
    {
        Left, Right, Continuous
    }
    public WalkOptions WalkOption;
    public float WalkDuration;
    public float WalkSpeed;

    private Walk walkAbility;

    public enum Outputs
    {
        StartWalk, EndWalk, HitWall
    }

    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        walkAbility = bossBehaviour.GetAbility<Walk>();
    }

    public override void ActivateBehaviour()
    {
        boss.GetComponent<Boss>().Walk();
        walkAbility.Activate(this, WalkDuration, WalkSpeed, WalkOption);
    }

    public void EndWalk()
    {
        boss.GetComponent<Boss>().EndWalk();
        CallNext((int)Outputs.EndWalk);
    }

    public void HitWall()
    {
        CallNext((int)Outputs.HitWall);
    }
}
