using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile
{
    private readonly Transform _owner;
    private const float DefaultProjectileSpeed = 7f;
    private const int NumProjectiles = 7;
    private readonly List<ProjectileType> _projectiles = new List<ProjectileType>();

    private int _projectileIndex;

    public struct ProjectileType
    {
        public Transform Tf;
        public Rigidbody2D Body;
        public float Speed;
    } 

    public StraightProjectile(Transform owner)
    {
        _owner = owner;
        CreateProjectilePool();
    }

    private void CreateProjectilePool()
    {
        var projectileParent = new GameObject("Projectiles").transform;
        for (int i = 0; i < NumProjectiles; i++)
        {
            var newProjectileObj = Object.Instantiate(Resources.Load<GameObject>("Projectile"));
            var newProjectile = new ProjectileType
            {
                Tf = newProjectileObj.transform,
                Body = newProjectileObj.GetComponent<Rigidbody2D>()
            };

            newProjectile.Tf.name = "Projectile";
            newProjectile.Tf.SetParent(projectileParent);
            newProjectile.Tf.position = Toolbox.Instance.HoldingArea;
            newProjectile.Body.isKinematic = true;

            _projectiles.Add(newProjectile);
        }
    }

    public void ActivateProjectile(float speed = DefaultProjectileSpeed)
    {
        _projectileIndex++;
        if (_projectileIndex == NumProjectiles) { _projectileIndex = 0; }
    
        var projectile = _projectiles[_projectileIndex];
        projectile.Tf.position = _owner.position;
        projectile.Speed = speed;
        projectile.Body.velocity = Vector2.left * speed;
        _projectiles[_projectileIndex] = projectile;
    }

    public void DeactivateProjectile(Rigidbody2D projectileBody)
    {
        projectileBody.velocity = Vector2.zero;
        projectileBody.position = Toolbox.Instance.HoldingArea;
    }

    public void PauseGame(bool paused)
    {
        foreach (var projectile in _projectiles)
        {
            projectile.Body.velocity = Vector2.left * (paused ? 0f : projectile.Speed);
        }
    }
}
