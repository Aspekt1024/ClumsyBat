using UnityEngine;

public class ProjectileAction : BaseAction {

    private ParabolicProjectile parProjectile;
    private Player player;

    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        parProjectile = (ParabolicProjectile)bossBehaviour.GetAbility<ParabolicProjectile>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public override void ActivateBehaviour()
    {
        parProjectile.ActivateProjectile(player.transform.position);
        CallNext();
    }
}
