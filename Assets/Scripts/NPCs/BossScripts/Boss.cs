using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour {
    
    protected Player _player;
    protected Rigidbody2D _body;
    protected Animator _anim;
    protected SpriteRenderer _renderer;
    protected Collider2D _collider;

    protected bool _bPaused;
    private Vector2 _storedVelocity;
    
    protected int _health;

    protected enum BossStates
    {
        Disabled,
        Idle,
        Jumping,
        Dead
    }

    protected BossStates _state;

    private void OnEnable()
    {
        SetEvents();
    }
    private void OnDisable()
    {
        RemoveEvents();
    }

    private void Awake()
    {
        _state = BossStates.Idle;
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _player = FindObjectOfType<Player>();
    }

    protected virtual void PauseGame()
    {
        _bPaused = true;
        _storedVelocity = _body.velocity;
        _body.velocity = Vector2.zero;
        _body.isKinematic = true;
        _anim.enabled = false;
    }

    protected virtual void ResumeGame()
    {
        _bPaused = false;
        _body.velocity = _storedVelocity;
        _body.isKinematic = false;
        _anim.enabled = true;
    }

    public virtual void HitByHypersonic() { }

    protected void TakeDamage(int damage = 1)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine("Damaged");
        }
    }

    protected virtual void Die()
    {
        _state = BossStates.Dead;
        _collider.enabled = false;
        //_anim.Play("Die");
        _body.velocity = new Vector2(3f, 5f);

        if (!_player.IsAlive()) return;
        EventListener.LevelWon();
    }

    private IEnumerator Damaged()
    {
        const int numFlashes = 8;
        const float flashDuration = 0.05f;

        Color originalColor = _renderer.color;

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
        _renderer.color = originalColor;
    }

    public bool IsPaused()
    {
        return _bPaused;
    }

    protected virtual void SetEvents()
    {
        EventListener.OnPauseGame += PauseGame;
        EventListener.OnResumeGame += ResumeGame;
    }

    protected virtual void RemoveEvents()
    {
        EventListener.OnPauseGame -= PauseGame;
        EventListener.OnResumeGame -= ResumeGame;
    }
}
