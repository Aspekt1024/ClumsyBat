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

    private Player player;

    public override void ActivateBehaviour()
    {
        switch(PlayerEvent)
        {
            case PlayerEvents.Stun:
                player.Stun(1.3f);
                break;
            case PlayerEvents.Damage:
                break;
            case PlayerEvents.GiveShield:
                break;
        }
    }

    public override void GameSetup(StateMachine owningContainer, BossData behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
}
