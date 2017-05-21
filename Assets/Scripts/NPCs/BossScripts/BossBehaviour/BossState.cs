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

    public void SetupActions(BossData behaviour, GameObject bossReference)
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
            RootStateMachine.CurrentAction.CallNext();
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
