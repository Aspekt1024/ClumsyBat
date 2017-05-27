﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// State Machine for the Boss
/// </summary>
[CreateAssetMenu(fileName = "BossData", menuName = "Custom/Boss", order = 1)]
public class StateMachine : BehaviourSet
{
    public string BossName;
    public GameObject BossPrefab; 
    public int Health;
    public bool SpawnMoths;    // TODO make into selectable list, per state
    public bool ShakeScreenOnLanding;
    
    public List<State> ActiveStates = new List<State>();
    
    public void StateMachineSetup(BossData bossData, GameObject boss)
    {
        BossActionLoadHandler.Load(this);

        foreach(var action in Actions)
        {
            action.GameSetup(this, bossData, boss);
        }
    }

    public void AwakenBoss()
    {
        IsEnabled = true;
        StartingAction.Activate();
    }

    public void ActivateNewState(State state)
    {
        ActiveStates.Add(state);
        state.IsEnabled = true;
    }

    public override void LoopToStart()
    {
        StartingAction.Activate();
    }

    public void HealthChanged(int health)
    {
        // TODO insert list of interrupt events
        // check if any of these are dependent on health, and what value

        // this will need to pause any current scripts and run the interrupt first
    }
}
