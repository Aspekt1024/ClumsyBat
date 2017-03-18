using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : BossAbility {
    
    private const float DefaultProjectileSpeed = 12.5f; // Todo could add variance / some randomness to this speed.
    private const int NumProjectiles = 400;
    private readonly List<ProjectileType> _projectiles = new List<ProjectileType>();

    private int _projectileIndex;

    public struct ProjectileType
    {
        public Transform Tf;
        public Rigidbody2D Body;
        public Collider2D Coll;
        public Vector2 PausedVelocity;
        public Projectile Script;
        public float PausedAngularVelocity;
    }

    private void Start()
    {
        CreateProjectilePool();
    }

    private void CreateProjectilePool()
    {
        var projectileParent = new GameObject("Rocks").transform;
        for (int i = 0; i < NumProjectiles; i++)
        {
            var newProjectileObj = Instantiate(Resources.Load<GameObject>("Projectiles/Rock"));
            var newProjectile = new ProjectileType
            {
                Tf = newProjectileObj.transform,
                Body = newProjectileObj.GetComponent<Rigidbody2D>(),
                Coll = newProjectileObj.GetComponent<Collider2D>(),
                Script = newProjectileObj.GetComponent<Projectile>()
            };

            newProjectile.Tf.name = "Rock";
            newProjectile.Tf.SetParent(projectileParent);
            newProjectile.Tf.position = Toolbox.Instance.HoldingArea;
            newProjectile.Body.isKinematic = false;
            newProjectile.Coll.enabled = false;
            newProjectile.Script.SetPoolOwner(this);

            _projectiles.Add(newProjectile);
        }
    }

    /// <summary>
    /// Fires a projectile at the player at a given (or default) speed.
    /// Returns true if throwing a projectile will hit the target position.
    /// </summary>
    public bool ActivateProjectile(ProjectileAction caller, Vector2 targetPos, float speed = DefaultProjectileSpeed)
    {
        _projectileIndex++;
        if (_projectileIndex == NumProjectiles) { _projectileIndex = 0; }
        var startPos = new Vector3(transform.position.x, transform.position.y, Toolbox.Instance.ZLayers["Projectile"]);
        
        Vector2 velocity = CalculateThrowingVelocity(startPos, targetPos, caller.TargetGround, speed);
        if (Mathf.Abs(velocity.x) < 0.0001f) { return false; }
        
        var projectile = _projectiles[_projectileIndex];
        projectile.Tf.position = startPos;
        projectile.Body.velocity = velocity;
        projectile.Coll.enabled = true;
        projectile.Body.AddTorque(200f);    // TODO could randomise this torque
        projectile.Script.SetCaller(caller);
        _projectiles[_projectileIndex] = projectile;

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
        float g = -Physics2D.gravity.y;
        float x = startPos.x - playerPos.x;
        float y = playerPos.y - startPos.y;

        bool backwards = x < 0;
        if (backwards) x = -x;

        // Source: https://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Angle_required_to_hit_coordinate_.28x.2Cy.29
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
        for (int i = 0; i < _projectiles.Count; i++)
        {
            var projectile = _projectiles[i];
            projectile.PausedVelocity = projectile.Body.velocity;
            projectile.PausedAngularVelocity = projectile.Body.angularVelocity;
            projectile.Body.velocity = Vector2.zero;
            projectile.Body.angularVelocity = 0f;
            projectile.Body.isKinematic = true;
            _projectiles[i] = projectile;
        }
    }

    public override void Resume()
    {
        for (int i = 0; i < _projectiles.Count; i++)
        {
            var projectile = _projectiles[i];
            projectile.Body.isKinematic = false;
            projectile.Body.velocity = projectile.PausedVelocity;
            projectile.Body.angularVelocity = projectile.PausedAngularVelocity;
            _projectiles[i] = projectile;
        }
    }
}
