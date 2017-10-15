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
        Input, Direction,
        StartWalk, EndWalk, HitWall
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        walkAbility = base.bossData.GetAbility<Walk>();
    }

    protected override void ActivateBehaviour()
    {
        if (GetInterface((int)Ifaces.Direction).IsConnected())
        {
            if (boss.transform.position.x > GetInterface((int)Ifaces.Direction).ConnectedInterface.Action.GetPosition(GetInterface((int)Ifaces.Direction).OtherConnID).x)
            {
                WalkOption = WalkOptions.Left;
                boss.GetComponent<Boss>().FaceDirection(Boss.Direction.Left);
            }
            else
            {
                boss.GetComponent<Boss>().FaceDirection(Boss.Direction.Right);
                WalkOption = WalkOptions.Right;
            }

        }
        boss.GetComponent<Boss>().Walk();   // Used for animations
        walkAbility.Activate(this, WalkDuration, WalkSpeed, WalkOption);
    }

    public void EndWalk()
    {
        if (!IsActive) return;
        IsActive = false;
        boss.GetComponent<Boss>().EndWalk();
        CallNext((int)Ifaces.EndWalk);
    }

    public void HitWall()
    {
        CallNext((int)Ifaces.HitWall);
    }

    public override void Stop()
    {
        IsStopped = true;
        if (!IsActive) return;

        IsActive = false;
        boss.GetComponent<Boss>().EndWalk();
        walkAbility.Interrupt();
    }
}
