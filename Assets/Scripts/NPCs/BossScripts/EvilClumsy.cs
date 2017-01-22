using System.Collections;
using UnityEngine;

public class EvilClumsy : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _body;
    private Animator _anim;
    private SpriteRenderer _renderer;
    private CircleCollider2D _collider;
    private Projectile _projectile;

    private bool _bPaused;
    private Vector2 _storedVelocity;

    private const float ProjectileCooldown = 1f;
    private float _projectileTimer = 3f;

    private int _health;

    private enum BossStates
    {
        Idle,
        Jumping,
        Dead
    }

    private BossStates _state;

	private void Start () {
	    _state = BossStates.Idle;
	    _body = GetComponent<Rigidbody2D>();
	    _anim = GetComponent<Animator>();
	    _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
        _player = FindObjectOfType<Player>();
        _projectile = new Projectile(transform);
        _health = 3;
    }
	
	private void Update () {

        if (_state == BossStates.Dead || _bPaused) { return; }

	    FollowTargetPosition();
	
	}

    private void FollowTargetPosition()
    {
        float targetY = _player.IsAlive() ? _player.transform.position.y : 0f;

        if (_player.IsAlive())
        {
            _projectileTimer -= Time.deltaTime;
            FireIfReady();
        }

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

    public void PauseGame(bool paused)
    {
        _bPaused = paused;
        if (paused)
        {
            _storedVelocity = _body.velocity;
            _body.velocity = Vector2.zero;
        }
        else
        {
            _body.velocity = _storedVelocity;
        }
        _body.isKinematic = paused;
        _anim.enabled = !paused;
        _projectile.PauseGame(paused);
    }

    public void TakeDamage(int damage = 1)
    {
        _health -= damage;
        if (_health <= 0)
        {
            _state = BossStates.Dead;
            _collider.enabled = false;
            transform.localScale *= 4;
            transform.rotation = Quaternion.identity;
            _anim.Play("Die");
            _body.velocity = new Vector2(3f, 5f);
            FindObjectOfType<BossGameHandler>().LevelComplete();
        }
        else
        {
            StartCoroutine("Damaged");
        }
    }

    private IEnumerator Damaged()
    {
        const int numFlashes = 8;
        const float flashDuration = 0.05f;

        bool flashOn = true;

        for (var i = 0; i < numFlashes; i++)
        {
            _renderer.color = flashOn ? new Color(1f, 0.7f, 0.7f) : new Color(1f, 0f, 0f);
            flashOn = !flashOn;

            float flashTimer = 0f;
            while (flashTimer < flashDuration)
            {
                if (!_bPaused)
                {
                    flashTimer += Time.deltaTime;
                }
                yield return null;
            }
        }
        _renderer.color = new Color(1f, 0f, 0f);
    }
}
