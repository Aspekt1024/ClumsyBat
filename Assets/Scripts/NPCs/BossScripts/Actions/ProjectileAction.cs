using UnityEngine;

public class ProjectileAction : BaseAction {

    public enum Outputs
    {
        Launched, Landed, HitPlayer,
        Projectile
    }

    private ParabolicProjectile parProjectile;
    private Player player;
    private Projectile projectileObj;

    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        parProjectile = bossBehaviour.GetAbility<ParabolicProjectile>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public override void ActivateBehaviour()
    {
        parProjectile.ActivateProjectile(this);
    }

    public void Launched()
    {
        CallNext((int)Outputs.Launched);
    }

    public void HitPlayer()
    {
        CallNext((int)Outputs.HitPlayer);
    }

    public void Landed(Projectile projectile)
    {
        projectileObj = projectile;
        CallNext((int)Outputs.Landed);
    }

    public override GameObject GetObject(int id)
    {
        return projectileObj.gameObject;
    }
}
