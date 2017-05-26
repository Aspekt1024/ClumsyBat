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

    public enum Ifaces
    {
        Input,
        StartWalk, EndWalk, HitWall
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        walkAbility = base.bossData.GetAbility<Walk>();
    }

    public override void ActivateBehaviour()
    {
        boss.GetComponent<Boss>().Walk();
        walkAbility.Activate(this, WalkDuration, WalkSpeed, WalkOption);
    }

    public void EndWalk()
    {
        boss.GetComponent<Boss>().EndWalk();
        CallNext((int)Ifaces.EndWalk);
    }

    public void HitWall()
    {
        CallNext((int)Ifaces.HitWall);
    }
}
