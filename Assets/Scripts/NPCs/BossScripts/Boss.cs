using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour {

    protected Player _player;
    private Rigidbody2D _body;
    private Animator _anim;
    private SpriteRenderer _renderer;
    private CircleCollider2D _collider;

    protected bool _bPaused;
    private Vector2 _storedVelocity;
    
    protected int _health;

    protected enum BossStates
    {
        Idle,
        Jumping,
        Dead
    }

    protected BossStates _state;

    private void Awake()
    {
        _state = BossStates.Idle;
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
        _player = FindObjectOfType<Player>();
    }

    public virtual void PauseGame(bool paused)
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
