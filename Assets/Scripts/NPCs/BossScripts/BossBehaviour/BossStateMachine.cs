using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// State Machine for the Boss
/// </summary>
[CreateAssetMenu(fileName = "BossData", menuName = "Custom/Boss", order = 1)]
public class BossStateMachine : StateMachine
{
    public GameObject BossPrefab; 
    public int Health;
    public bool SpawnMoths;    // TODO make into selectable list, per state
    public bool ShakeScreenOnLanding;
    
    public BossState CurrentState;
    public StartAction LastStartingAction;
    
    private GameObject bossObject;

    public void NodeGameSetup(BossData bossBehaviour, GameObject boss)
    {
        Toolbox.Instance.GamePaused = false;    // TODO shouldnt be done here...
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
