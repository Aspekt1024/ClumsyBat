using ClumsyBat;
using UnityEngine;

public class PlayerEventAction : BaseAction {

    public enum PlayerEvents
    {
        Stun, GiveShield, Damage
    }
    public PlayerEvents PlayerEvent;

    protected override void ActivateBehaviour()
    {
        IsActive = false;

        switch(PlayerEvent)
        {
            case PlayerEvents.Stun:
                GameStatics.Player.Clumsy.Stun(1.3f);
                break;
            case PlayerEvents.Damage:
                GameStatics.Player.Clumsy.TakeDamage("Boss");
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
