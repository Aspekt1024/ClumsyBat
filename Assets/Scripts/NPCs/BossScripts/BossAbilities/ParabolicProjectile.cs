using System.Collections.Generic;
using UnityEngine;

public class ParabolicProjectile {

    private readonly Transform _owner;
    private const float DefaultProjectileSpeed = 12.5f;
    private const int NumProjectiles = 4;
    private readonly List<ProjectileType> _projectiles = new List<ProjectileType>();

    private int _projectileIndex;

    public struct ProjectileType
    {
        public Transform Tf;
        public Rigidbody2D Body;
        public Vector2 PausedVelocity;
    }

    public ParabolicProjectile(Transform owner)
    {
        _owner = owner;
        CreateProjectilePool();
    }

    private void CreateProjectilePool()
    {
        var projectileParent = new GameObject("Rocks").transform;
        for (int i = 0; i < NumProjectiles; i++)
        {
            var newProjectileObj = Object.Instantiate(Resources.Load<GameObject>("Projectiles/Rock"));
            var newProjectile = new ProjectileType
            {
                Tf = newProjectileObj.transform,
                Body = newProjectileObj.GetComponent<Rigidbody2D>()
            };

            newProjectile.Tf.name = "Rock";
            newProjectile.Tf.SetParent(projectileParent);
            newProjectile.Tf.position = Toolbox.Instance.HoldingArea;
            newProjectile.Body.isKinematic = false;

            _projectiles.Add(newProjectile);
        }
    }

    /// <summary>
    /// Fires a projectile at the player at a given (or default) speed.
    /// Returns true if throwing a projectile will hit the player position.
    /// </summary>
    public bool ActivateProjectile(Vector3 playerPos, float speed = DefaultProjectileSpeed)
    {
        _projectileIndex++;
        if (_projectileIndex == NumProjectiles) { _projectileIndex = 0; }
        var startPos = _owner.position;
        
        Vector2 velocity = CalculateThrowingVelocity(startPos, playerPos, speed);
        if (Mathf.Abs(velocity.x) < 0.0001f) { return false; }
        
        var projectile = _projectiles[_projectileIndex];
        projectile.Tf.position = startPos;
        projectile.Body.velocity = velocity;
        projectile.Body.AddTorque(200f);
        _projectiles[_projectileIndex] = projectile;
        return true;
    }

    private Vector2 CalculateThrowingVelocity(Vector3 startPos, Vector3 playerPos, float speed)
    {
        float gravity = -Physics2D.gravity.y;
        float horizontalDist = startPos.x - playerPos.x;
        float verticalDist = playerPos.y - startPos.y;

        // Source: https://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Angle_required_to_hit_coordinate_.28x.2Cy.29
        float angle = Mathf.Atan((speed * speed - Mathf.Sqrt(Mathf.Pow(speed, 4) - gravity * (gravity * horizontalDist * horizontalDist + 2 * verticalDist * speed * speed))) / (gravity * horizontalDist));
        if (float.IsNaN(angle))
        {
            return Vector2.zero;
        }

        float horizontalSpeed = Mathf.Cos(angle);
        float verticalSpeed = Mathf.Sin(angle);
        Vector2 velocity = new Vector2(-Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

        return velocity;
    }

    public void DeactivateProjectile(Rigidbody2D projectileBody)
    {
        projectileBody.velocity = Vector2.zero;
        projectileBody.position = Toolbox.Instance.HoldingArea;
    }

    // TODO split this into Pause/resume functions
    public void PauseGame(bool paused)
    {
        if (paused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    private void Pause()
    {
        for (int i = 0; i < _projectiles.Count; i++)
        {
            var projectile = _projectiles[i];
            projectile.PausedVelocity = projectile.Body.velocity;
            projectile.Body.velocity = Vector2.zero;
            _projectiles[i] = projectile;
        }
    }

    private void Resume()
    {
        for (int i = 0; i < _projectiles.Count; i++)
        {
            var projectile = _projectiles[i];
            projectile.Body.velocity = projectile.PausedVelocity;
            _projectiles[i] = projectile;
        }
    }
}
