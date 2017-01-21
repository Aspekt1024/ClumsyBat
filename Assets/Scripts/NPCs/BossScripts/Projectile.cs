using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Projectile
{
    private readonly Transform _parent;
    private const float DefaultProjectileSpeed = 4f;
    private const int NumProjectiles = 3;
    private readonly List<ProjectileType> _projectiles = new List<ProjectileType>();

    private int _projectileIndex;

    private bool _bPaused;

    public struct ProjectileType
    {
        public Transform Tf;
        public Rigidbody2D Body;
        public float Speed;
    }

    public Projectile(Transform owner)
    {
        _parent = owner;
        CreateProjectilePool();
    }

    private void CreateProjectilePool()
    {
        for (int i = 0; i < NumProjectiles; i++)
        {
            var parent = new GameObject("Projectiles");
            var newProjectileObj = Object.Instantiate(Resources.Load<GameObject>("Projectile"));
            var newProjectile = new ProjectileType
            {
                Tf = newProjectileObj.transform,
                Body = newProjectileObj.GetComponent<Rigidbody2D>()
            };

            newProjectile.Tf.name = "Projectile";
            newProjectile.Tf.SetParent(parent.transform);
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
        projectile.Tf.position = _parent.position;
        projectile.Speed = speed;
        projectile.Body.velocity = Vector2.left * projectile.Speed;
        _projectiles[_projectileIndex] = projectile;
    }

    public void DeactivateProjectile(Rigidbody2D projectileBody)
    {
        projectileBody.velocity = Vector2.zero;
        projectileBody.position = Toolbox.Instance.HoldingArea;
    }

    public void PauseGame(bool paused)
    {
        _bPaused = paused;
        foreach (var projectile in _projectiles)
        {
            projectile.Body.velocity = Vector2.left * (paused ? 0f : projectile.Speed);
        }
    }
}
