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

    }

    public void AwakenBoss()
    {

    }

    private void ActivateStateIfStateNode(BaseAction action)
    {

    }
}
