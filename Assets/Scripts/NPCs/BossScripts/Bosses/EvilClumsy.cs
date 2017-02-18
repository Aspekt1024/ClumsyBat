using UnityEngine;

public class EvilClumsy : Boss
{
    private StraightProjectile _projectile;

    private const float ProjectileCooldown = 1f;
    private float _projectileTimer = 3f;

	private void Start ()
    {
        _projectile = new StraightProjectile(transform);
        _health = 3;
    }
	
	private void Update () {

        if (_state == BossStates.Dead || _bPaused) { return; }

	    FollowTargetPosition();

        if (_player.IsAlive())
        {
            _projectileTimer -= Time.deltaTime;
            FireIfReady();
        }
    }

    private void FollowTargetPosition()
    {
        float targetY = _player.IsAlive() ? _player.transform.position.y : 0f;
        
        if (_state == BossStates.Jumping)
        {
            if (_body.velocity.y < 0)
            {
                _state = BossStates.Idle;
            }
        }

        if (transform.position.y >= targetY || _state != BossStates.Idle) { return; }
        _body.velocity = Vector2.zero;
        _body.AddForce(new Vector2(0f, 450f));
        _state = BossStates.Jumping;
        _anim.Play("Flap", 0, 0f);
    }

    private void FireIfReady()
    {
        if (!(_projectileTimer <= 0f)) return;
        _projectileTimer = ProjectileCooldown;
        _projectile.ActivateProjectile();
    }
}
