using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : BossAbility {
    
    private const float DefaultProjectileSpeed = 12.5f;
    private readonly List<Projectile> _projectiles = new List<Projectile>();
    
    private Transform projectileParent;
    private GameObject rockReference;
    private GameObject mothCrystalReference;
    
    private void Start()
    {
        rockReference = Resources.Load<GameObject>("Projectiles/Rock");
        mothCrystalReference = Resources.Load<GameObject>("Projectiles/MothCrystal");
        projectileParent = new GameObject("Rocks").transform;
    }

    /// <summary>
    /// Fires a projectile at the player at a given (or default) speed.
    /// Returns true if throwing a projectile will hit the target position.
    /// </summary>
    public bool ActivateProjectile(ProjectileAction caller, Vector2 targetPos, float speed = DefaultProjectileSpeed)
    {
        var startPos = new Vector3(transform.position.x, transform.position.y, Toolbox.Instance.ZLayers["Projectile"]);
        Vector2 velocity = CalculateThrowingVelocity(startPos, targetPos, caller.TargetGround, speed);

        if (Mathf.Abs(velocity.x) < 0.0001f) return false;

        Projectile newProjectile = null;
        switch(caller.ProjectileType)
        {
            case ProjectileAction.ProjectileTypes.Rock:
                newProjectile = Instantiate(rockReference).GetComponent<Projectile>();
                break;
            case ProjectileAction.ProjectileTypes.MothCrystal:
                newProjectile = Instantiate(mothCrystalReference).GetComponent<Projectile>();
                ((MothCrystal)newProjectile).SetColour(caller.MothColour);
                break;
            default:
                Debug.LogError("Invalid Projectile Type called by ActivateProjectile() in ProjectileAbility");
                break;
        }
        
        newProjectile.transform.SetParent(projectileParent);
        newProjectile.transform.position = startPos;
        newProjectile.Body.velocity = velocity;
        newProjectile.Collider.enabled = true;
        newProjectile.Body.AddTorque(Random.Range(150f, 200f));
        newProjectile.SetReferences(caller, this);

        _projectiles.Add(newProjectile);

        caller.Launched();

        return true;
    }

    private Vector2 CalculateThrowingVelocity(Vector3 startPos, Vector3 playerPos, bool targetGround, float speed)
    {
        float angle = CalculateThrowingAngle(startPos, playerPos, targetGround, speed);
        if (Mathf.Abs(angle) < 0.001f)
        {
            return Vector2.zero;
        }

        float horizontalSpeed = Mathf.Cos(angle);
        float verticalSpeed = Mathf.Sin(angle);
        Vector2 velocity = new Vector2(-horizontalSpeed, verticalSpeed) * speed;

        return velocity;
    }

    private float CalculateThrowingAngle(Vector3 startPos, Vector3 playerPos, bool upperPath, float s)
    {
        // Source: https://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Angle_required_to_hit_coordinate_.28x.2Cy.29
        float g = -Physics2D.gravity.y;
        float x = startPos.x - playerPos.x;
        float y = playerPos.y - startPos.y;

        bool backwards = x < 0;
        if (backwards) x = -x;

        float angle;
        if (upperPath)
            angle = Mathf.Atan((s * s + Mathf.Sqrt(Mathf.Pow(s, 4) - g * (g * x * x + 2 * y * s * s))) / (g * x));
        else
            angle = Mathf.Atan((s * s - Mathf.Sqrt(Mathf.Pow(s, 4) - g * (g * x * x + 2 * y * s * s))) / (g * x));

        if (float.IsNaN(angle)) angle = 0;
        return backwards ? Mathf.PI - angle : angle;
    }

    public override void Pause()
    {
        foreach (Projectile projectile in _projectiles)
        {
            projectile.Pause();
        }
    }

    public override void Resume()
    {
        foreach (Projectile projectile in _projectiles)
        {
            projectile.Resume();
        }
    }

    public void RemoveProjectile(Projectile projectile)
    {
        _projectiles.Remove(projectile);
    }
}
