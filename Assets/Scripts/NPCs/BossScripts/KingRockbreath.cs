public class KingRockbreath : Boss
{
    private Projectile _projectile;

    private const float ProjectileCooldown = 1f;
    private float _projectileTimer = 3f;
    
    private void Start()
    {
        _projectile = new Projectile(transform);
        _health = 1;
    }

    private void Update()
    {
        if (_state == BossStates.Dead || _bPaused) { return; }
    }

    private void FireIfReady()
    {
        if (!(_projectileTimer <= 0f)) return;
        _projectileTimer = ProjectileCooldown;
        _projectile.ActivateProjectile();
    }
}
