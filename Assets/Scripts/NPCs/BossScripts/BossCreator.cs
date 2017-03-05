using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossData", menuName = "Custom/Boss", order = 1)]
public class BossCreator : ScriptableObject
{
    public string BossName;
    public GameObject BossPrefab; 
    public int Health;
    public bool bSpawnMoths;    // TODO make into selectable list, per state
    
    public List<BaseAction> Actions = new List<BaseAction>();
    public List<BossState> States = new List<BossState>();

    public BossState CurrentState;

    public void NodeGameSetup(BossBehaviour behaviour, GameObject boss)
    {
        foreach (var state in States)
        {
            foreach (var action in state.Actions)
            {
                action.GameSetup(behaviour, boss);
            }
        }
    }

    public void AwakenBoss()
    {
        foreach(var action in Actions)
        {
            Debug.Log("action " + action);
            if (action.GetType().Equals(typeof(StartAction)))
                ActivateStateIfStateNode(action.GetNextAction());
        }
    }

    private void ActivateStateIfStateNode(BaseAction action)
    {
        Debug.Log(action);
        if (!action.GetType().Equals(typeof(MachineState))) return;

        CurrentState = ((MachineState)action).State;

        foreach (var a in CurrentState.Actions)
        {
            if (a.GetType().Equals(typeof(StartAction)))
                a.Activate();
        }
    }
}
