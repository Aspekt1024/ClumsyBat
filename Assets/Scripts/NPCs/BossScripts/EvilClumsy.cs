using UnityEngine;

public class EvilClumsy : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _body;
    private Animator _anim;
    private Projectile _projectile;

    private bool _bPaused;
    private Vector2 _storedVelocity;

    private const float ProjectileCooldown = 2f;
    private float _projectileTimer = ProjectileCooldown;
    private float _projectileSpeed = 7f;

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
	    _player = FindObjectOfType<Player>();
        _projectile = new Projectile(transform);
	}
	
	private void Update () {

        if (_state == BossStates.Dead || _bPaused) { return; }

	    _projectileTimer -= Time.deltaTime;
	    FireIfReady();

	    if (_state == BossStates.Jumping)
	    {
	        if (_body.velocity.y < 0)
	        {
	            _state = BossStates.Idle;
	        }
	    }

	    if (transform.position.y >= _player.transform.position.y || _state != BossStates.Idle) { return; }
	    _body.velocity = Vector2.zero;
	    _body.AddForce(new Vector2(0f, 450f));
        _state = BossStates.Jumping;
        _anim.Play("Flap", 0, 0f);
	
	}

    private void FireIfReady()
    {
        if (!(_projectileTimer <= 0f)) return;
        _projectileTimer = ProjectileCooldown;
        _projectile.ActivateProjectile(_projectileSpeed);
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
}
