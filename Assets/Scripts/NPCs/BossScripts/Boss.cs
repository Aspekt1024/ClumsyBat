using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for describing specific boss behaviour
/// </summary>
public abstract class Boss : MonoBehaviour {
    
    // TODO put this in a helper class
    public enum Direction
    {
        Left, Right, Switch
    }

    public Rigidbody2D Body;

    protected bool _bPaused;
    protected int health;
    protected SpriteRenderer bossRenderer;
    protected Collider2D bossCollider;

    private StateMachine machine;

    private Vector2 originalScale;  // Used for facing the boss left/right
    private Vector2 storedVelocity;
    private float damageCooldownTimer;
    
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
        GetBossComponents();
        originalScale = transform.localScale;   // Boss should be facing left first
    }

    private void Update()
    {
        if (damageCooldownTimer < 0) return;
        damageCooldownTimer -= Time.deltaTime;
    }

    protected abstract void GetBossComponents();

    public void SetBaseProperties(StateMachine stateMachine)
    {
        machine = stateMachine;
        health = stateMachine.Health;
    }

    protected virtual Rigidbody2D GetRigidBody()
    {
        return GetComponent<Rigidbody2D>();
    }

    protected virtual void PauseGame()
    {
        _bPaused = true;
        storedVelocity = Body.velocity;
        Body.velocity = Vector2.zero;
        Body.isKinematic = true;
    }

    protected virtual void ResumeGame()
    {
        _bPaused = false;
        Body.velocity = storedVelocity;
        Body.isKinematic = false;
    }

    public void TriggerEnter(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
    public void CollisionEnter(Collision2D other)
    {
        OnCollisionEnter2D(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageCooldownTimer > 0) return;
        if (other.name == "HypersonicMask")
        {
            machine.Damaged(CollisionAction.CollisionTypes.Hypersonic, other);
            damageCooldownTimer = 1f;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Stalactite")
        {
            if (other.collider.gameObject.GetComponent<Stalactite>().IsFalling())
                machine.Damaged(CollisionAction.CollisionTypes.FallingStalactite, other.collider);
            else
                machine.Damaged(CollisionAction.CollisionTypes.StaticStalactite, other.collider);
        }
        else if (other.collider.tag == "Player")
        {
            if (Toolbox.Player.IsRushing())
                machine.Damaged(CollisionAction.CollisionTypes.Dash, other.collider);
            else
                machine.Damaged(CollisionAction.CollisionTypes.Player, other.collider);
        }
    }

    public virtual void FaceDirection(Direction dir)
    {
        // This assumes the boss is facing to the left when scale.x is positive
        Transform bossParent = GetComponentInParent<Transform>();
        bool switchDir = false;
        switch(dir)
        {
            case Direction.Left:
                if (bossParent.localScale.x < 0) switchDir = true;
                break;

            case Direction.Right:
                if (bossParent.localScale.x > 0) switchDir = true;
                break;

            case Direction.Switch:
                switchDir = true;
                break;
        }

        Vector3 scale = bossParent.localScale;
        if (switchDir)
            bossParent.localScale = new Vector3(-scale.x, scale.y, scale.z);
    }
    
    public void TakeDamage(int damage = 1)
    {
        if (this == null) return;

        machine.Stop();

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Damaged());
        }
        machine.HealthChanged(health);
        HealthUpdate();
    }

    protected virtual void HealthUpdate() { }

    protected virtual void Die()
    {
        bossCollider.enabled = false;
        Body.velocity = new Vector2(3f, 5f);
        //_anim.Play("Die");
        BossEvents.BossDeath();
    }

    private IEnumerator Damaged()
    {
        const int numFlashes = 8;
        const float flashDuration = 0.05f;
        
        bool flashOn = true;

        for (var i = 0; i < numFlashes; i++)
        {
            bossRenderer.color = flashOn ? new Color(1f, 0.7f, 0.7f) : new Color(1f, 0f, 0f);
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
        bossRenderer.color = Color.white;
        machine.StartingAction.Activate();
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

    public virtual void Walk() { }
    public virtual void EndWalk() { }
    public virtual void Jump() { }
    public virtual void EndJump() { }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }
}
