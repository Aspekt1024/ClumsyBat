using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventAction : BaseAction {

    public enum PlayerEvents
    {
        Stun, GiveShield, Damage
    }
    [SerializeField]
    public PlayerEvents PlayerEvent;
    
    public override void ActivateBehaviour()
    {
        switch(PlayerEvent)
        {
            case PlayerEvents.Stun:
                Toolbox.Player.Stun(1.3f);
                break;
            case PlayerEvents.Damage:
                break;
            case PlayerEvents.GiveShield:
                break;
        }
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
    }
}
