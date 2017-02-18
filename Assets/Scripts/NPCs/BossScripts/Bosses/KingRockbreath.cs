using UnityEngine;

public class KingRockbreath : Boss
{
    private ParabolicProjectile _projectile;

    private const float ProjectileCooldown = 2f;
    private float _projectileTimer = 3f;
    
    private void Start()
    {
        _projectile = new ParabolicProjectile(transform);
        _health = 3;
    }

    private void Update()
    {
        if (_state == BossStates.Dead || _bPaused || !_player.IsAlive()) { return; }
        _projectileTimer -= Time.deltaTime;
        FireIfReady();
    }

    private void FireIfReady()
    {
        if (!(_projectileTimer <= 0f)) return;
        bool throwPossible = _projectile.ActivateProjectile(_player.transform.position);
        if (throwPossible) { _projectileTimer = ProjectileCooldown; }
    }
}
