using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossData", menuName = "Custom/Boss", order = 1)]
public class BossCreator : BossDataContainer
{
    public GameObject BossPrefab; 
    public int Health;
    public bool bSpawnMoths;    // TODO make into selectable list, per state
    
    public BossState CurrentState;

    public void NodeGameSetup(BossBehaviour behaviour, GameObject boss)
    {
        foreach (var action in Actions)
        {
            if (action.GetType().Equals(typeof(MachineState)))
            {
                foreach (var a in ((MachineState)action).State.Actions)
                {
                    a.GameSetup(behaviour, boss);
                }
            }
        }
    }

    public void AwakenBoss()
    {
        foreach(var action in Actions)
        {
            if (action.GetType().Equals(typeof(StartAction)))
                ActivateStateIfStateNode(action.GetNextAction());
        }
    }

    private void ActivateStateIfStateNode(BaseAction action)
    {
        if (!action.GetType().Equals(typeof(MachineState))) return;

        CurrentState = ((MachineState)action).State;

        foreach (var a in CurrentState.Actions)
        {
            if (a.GetType().Equals(typeof(StartAction)))
                a.Activate();
        }
    }
}
