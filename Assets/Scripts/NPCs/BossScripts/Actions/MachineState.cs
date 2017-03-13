using UnityEngine;
using System.Collections.Generic;

public class MachineState : BaseAction {
    
    public enum BossDamageObjects
    {
        Hypersonic,
        Stalactite,
        Player
    }
    
    public BossState State;

    //public List<BossDamageObjects> damageObjects = new List<BossDamageObjects>(); // TODO could use this here instead
    
    public override void ActivateBehaviour()
    {
        bossBehaviour.BossProps.AssignNewState(State);
        State.StartingAction.Activate();
    }

    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        State.SetupActions(behaviour, bossReference);
    }

    public override void Tick(float deltaTime)
    {
        State.CurrentAction.Tick(deltaTime);
    }

}
