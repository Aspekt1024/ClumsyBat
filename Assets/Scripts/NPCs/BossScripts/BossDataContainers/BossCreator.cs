﻿using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossData", menuName = "Custom/Boss", order = 1)]
public class BossCreator : BossDataContainer
{
    public GameObject BossPrefab; 
    public int Health;
    public bool SpawnMoths;    // TODO make into selectable list, per state
    public bool ShakeScreenOnLanding;
    
    public BossState CurrentState;
    public StartAction LastStartingAction;

    private BossBehaviour behaviour;
    private GameObject bossObject;

    public void NodeGameSetup(BossBehaviour bossBehaviour, GameObject boss)
    {
        Toolbox.Instance.GamePaused = false;    // TODO shouldnt be done here...
        behaviour = bossBehaviour;
        bossObject = boss;
        foreach(var action in Actions)
        {
            action.GameSetup(this, bossBehaviour, boss);
        }
    }

    public void AwakenBoss()
    {
        bEnabled = true;
        StartingAction.Activate();
        CurrentState.bEnabled = true;
    }

    public void AssignNewState(BossState state)
    {
        CurrentState = state;
        bossObject.GetComponent<Boss>().SetPropsFromState(state);
    }

    public override void RequestLoopToStart()
    {
        StartingAction.Activate();
    }

    public void HealthChanged(int health)
    {
        if (CurrentState.StateChange == BossState.StateChangeTypes.Health && health == CurrentState.MoveOnHP)
        {
            CurrentState.Stop();
            CurrentAction.CallNext();
        }
    }
}
