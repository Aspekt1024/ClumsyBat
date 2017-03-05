using UnityEngine;

public class ProjectileAction : BaseAction {

    private ParabolicProjectile parProjectile;
    private Player player;

    public override void GameSetup(BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(behaviour, bossReference);
        parProjectile = (ParabolicProjectile)bossBehaviour.GetAbility<ParabolicProjectile>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public override void Activate()
    {
        parProjectile.ActivateProjectile(player.transform.position);
        CallNext();
    }
}
