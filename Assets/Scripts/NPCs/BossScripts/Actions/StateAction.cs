using UnityEngine;
using System.Collections.Generic;

public class StateAction : BaseAction {
    
    public enum BossDamageObjects
    {
        Hypersonic, Stalactite, Player
    }
    
    public BossState State;

    //public List<BossDamageObjects> damageObjects = new List<BossDamageObjects>(); // TODO could use this here instead
    
    public override void ActivateBehaviour()
    {
        bossBehaviour.BossProps.AssignNewState(State);
        State.BeginState();
    }

    public override void GameSetup(StateMachine owningContainer, BossData behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        State.SetupActions(behaviour, bossReference);
        State.bEnabled = false;
    }

    public override void Tick(float deltaTime)
    {
        if (State.bEnabled)
        {
            State.Tick(deltaTime);
        }
    }

}
