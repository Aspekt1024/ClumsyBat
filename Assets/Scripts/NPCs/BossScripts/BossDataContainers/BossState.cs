using UnityEngine;

public class BossState : BossDataContainer {
    
    public string StateName = "State";
    
    public bool DamagedByHypersonic;
    public bool DamagedByStalactites;
    public bool DamagedByPlayer;

    public void SetupActions(BossBehaviour behaviour, GameObject bossReference)
    {
        foreach (var action in Actions)
        {
            action.GameSetup(this, behaviour, bossReference);
        }
    }

    public void RenameState(string newStateName)
    {
        // TODO not sure if this is required... this is a reminder to implement if required.
    }
}
