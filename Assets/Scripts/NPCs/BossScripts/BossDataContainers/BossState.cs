using UnityEngine;

public class BossState : BossDataContainer {
    
    public string StateName = "State";

    public enum StateChangeTypes
    {
        Never, Health, NumLoops, Time
    }
    public StateChangeTypes StateChange;
    public int MoveOnHP;
    public float MoveAfterSeconds;
    public int MoveOnLoops;
    
    public bool DamagedByHypersonic;
    public bool DamagedByStalactites;
    public bool DamagedByPlayer;

    private int numLoops;

    public void SetupActions(BossBehaviour behaviour, GameObject bossReference)
    {
        foreach (var action in Actions)
        {
            action.GameSetup(this, behaviour, bossReference);
        }
    }

    public void BeginState()
    {
        numLoops = 0;
        bEnabled = true;
        StartingAction.Activate();
    }

    public override void RequestLoopToStart()
    {
        numLoops++;
        if (StateChange == StateChangeTypes.NumLoops && numLoops > MoveOnLoops)
        {
            bEnabled = false;
            RootContainer.CurrentAction.CallNext();
        }
        else
        {
            StartingAction.Activate();
        }
    }
}
