using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossData", menuName = "Custom/Boss", order = 1)]
public class BossCreator : BossDataContainer
{
    public GameObject BossPrefab; 
    public int Health;
    public bool bSpawnMoths;    // TODO make into selectable list, per state
    
    public BossState CurrentState;
    public StartAction LastStartingAction;

    public void NodeGameSetup(BossBehaviour behaviour, GameObject boss)
    {
        Toolbox.Instance.GamePaused = false;    // TODO shouldnt be done here...
        foreach(var action in Actions)
        {
            action.GameSetup(behaviour, boss);
        }
    }

    public void AwakenBoss()
    {
        StartingAction.Activate();
    }

    private void ActivateStateIfStateNode(BaseAction action)
    {

    }
}
