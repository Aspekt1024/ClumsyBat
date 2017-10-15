using UnityEngine;

public class ProjectileAction : BaseAction {

    public bool TargetGround;
    public Vector2 TargetPos;
    public float ProjectileSpeed;
    public ProjectileTypes ProjectileType;
    public Moth.MothColour MothColour;
    
    public enum Ifaces
    {
        Main,
        ProjectileIn, Position,

        Launched, Landed, HitPlayer,
        Projectile
    }

    public enum ProjectileTypes
    {
        Rock, MothCrystal
    }

    private ProjectileAbility projAbility;
    private Projectile projectileObj;

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        projAbility = base.bossData.GetAbility<ProjectileAbility>();
    }

    protected override void ActivateBehaviour()
    {
        IsActive = false;
        Vector2 tarPos = CalculateTargetPos();
        bool projectileSuccess = projAbility.ActivateProjectile(this, tarPos, ProjectileSpeed);
        if (!projectileSuccess)
        {
            Launched();
        }
    }

    public void Launched()
    {
        CallNext((int)Ifaces.Launched);
    }

    public void HitPlayer()
    {
        CallNext((int)Ifaces.HitPlayer);
    }

    public void Landed(Projectile projectile)
    {
        projectileObj = projectile;
        CallNext((int)Ifaces.Landed);
    }

    public override GameObject GetObject(int id)
    {
        if (projectileObj == null) return null;
        return projectileObj.gameObject;
    }

    private Vector2 CalculateTargetPos()
    {
        Vector2 outputPos = Vector2.zero;
        outputPos.x = TargetPos.x + GameObject.FindGameObjectWithTag("MainCamera").transform.position.x;

        ActionConnection posInput = GetInterface((int)Ifaces.Position);
        if (posInput.IsConnected())
            outputPos = posInput.ConnectedInterface.Action.GetObject(posInput.ConnectedInterface.ID).transform.position;

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
