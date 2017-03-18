using UnityEngine;

public class ProjectileAction : BaseAction {

    public bool TargetGround;
    public Vector2 TargetPos;
    public float ProjectileSpeed;

    public enum Inputs
    {
        Main,
        Projectile, Position
    }
    public enum Outputs
    {
        Launched, Landed, HitPlayer,
        Projectile
    }

    private ParabolicProjectile parProjectile;
    private Player player;  // TODO remove if this isn't the default. It won't be if we introduce a player node.
    private Projectile projectileObj;

    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        parProjectile = bossBehaviour.GetAbility<ParabolicProjectile>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public override void ActivateBehaviour()
    {
        Vector2 tarPos = CalculateTargetPos();
        bool projectileSuccess = parProjectile.ActivateProjectile(this, tarPos, ProjectileSpeed);
        if (!projectileSuccess)
        {
            Launched();
        }
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

    private Vector2 CalculateTargetPos()
    {
        Vector2 outputPos = Vector2.zero;
        outputPos.x = TargetPos.x + GameObject.FindGameObjectWithTag("MainCamera").transform.position.x;
        if (TargetGround)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(outputPos.x, 0), Vector3.down, 10f, (1 << LayerMask.NameToLayer("BossFloor")));
            if (hit.collider != null)
                outputPos.y = hit.point.y + 0.2f;
            else
                outputPos.y = -5f;
        }
        return outputPos;
    }
}
