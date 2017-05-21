using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A state within the machine for the Boss
/// </summary>
public class BossState : StateMachine {
    
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

    [SerializeField]
    private int numLoops;

    public void SetupActions(BossData bossData, GameObject bossReference)
    {
        BossActionLoadHandler.Load(this);
        foreach (var action in Actions)
        {
            action.GameSetup(this, bossData, bossReference);
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
            StartingAction.Activate();
            //RootStateMachine.CurrentAction.CallNext(); // TODO this is no longer used. use events instead
            // We will also be re-doing the Loop node to hold this data
        }
        else
        {
            StartingAction.Activate();
        }
    }

    public override void Tick(float deltaTime)
    {
        foreach (var action in Actions)
        {
            action.Tick(deltaTime);
        }
    }
}
