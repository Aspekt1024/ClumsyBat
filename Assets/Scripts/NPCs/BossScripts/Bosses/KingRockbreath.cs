using UnityEngine;

public class KingRockbreath : Boss
{
    private ParabolicProjectile _projectile;
    private JumpPound _jumpPound;

    private const float ProjectileCooldown = 2f;
    private float _projectileTimer = 3f;
    private const float JumpCooldown = 5f;
    private float _jumpTimer = 5f;

    private enum CustomBossState
    {
        ThrowingRock,
        JumpPound,
        Moving,
        Idle
    }
    private CustomBossState _cState = CustomBossState.Idle;
    
    private void Start()
    {
        _health = 4;
        LoadAbilities();
    }

    private void Update()
    {
        if (_state == BossStates.Dead || _bPaused || !_player.IsAlive()) { return; }
        SubtractCooldownTime(Time.deltaTime);
        ActivateAvailableAbility();
    }

    private void ThrowProjectile()
    {
        bool throwPossible = _projectile.ActivateProjectile(_player.transform.position);
        if (throwPossible) { _projectileTimer = ProjectileCooldown; }
    }

    private void JumpPound()
    {
        _jumpPound.Activate();
        _jumpTimer = JumpCooldown;
    }

    private void LoadAbilities()
    {
        _projectile = new ParabolicProjectile(transform);
        _jumpPound = gameObject.AddComponent<JumpPound>();
    }

    private void SubtractCooldownTime(float time)
    {
        _projectileTimer -= time;
        _jumpTimer -= time;
    }

    private void ActivateAvailableAbility()
    {
        if (_cState == CustomBossState.Idle)
        {
            if (_jumpTimer <= 0f)
                JumpPound();
            else if (_projectileTimer <= 0)
                ThrowProjectile();
        }
    }
}
