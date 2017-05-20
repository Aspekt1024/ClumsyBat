using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using BossDamageObjects = StateAction.BossDamageObjects;

/// <summary>
/// Base class for describing specific boss behaviour
/// </summary>
public class Boss : MonoBehaviour {
    
    protected bool _bPaused;
    protected int health;
    protected SpriteRenderer bossRenderer;
    protected Rigidbody2D body;
    protected Collider2D bossCollider;

    private BossStateMachine bossProps;

    private Vector2 originalScale;  // Used for facing the boss left/right
    private Vector2 storedVelocity;
    private float damageCooldownTimer;
    
    private List<BossDamageObjects> damageObjects = new List<BossDamageObjects>();

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
        Debug.Log("boss started");
        GetBossComponents();
        originalScale = transform.localScale;   // Boss should be facing left first
    }

    private void Update()
    {
        if (damageCooldownTimer < 0) return;
        damageCooldownTimer -= Time.deltaTime;
    }

    protected virtual void GetBossComponents()
    {
        body = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<Collider2D>();
        bossRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBaseProperties(BossStateMachine props)
    {
        bossProps = props;
        health = props.Health;
    }

    public void SetPropsFromState(BossState state)
    {
        damageObjects = new List<BossDamageObjects>();
        gameObject.layer = LayerMask.NameToLayer("Boss");
        if (state.DamagedByHypersonic)
        {
            damageObjects.Add(BossDamageObjects.Hypersonic);
        }
        if (state.DamagedByPlayer)
        {
            damageObjects.Add(BossDamageObjects.Player);
        }
        if (state.DamagedByStalactites)
        {
            damageObjects.Add(BossDamageObjects.Stalactite);
            bossCollider.gameObject.layer = LayerMask.NameToLayer("BossStalactite");
        }
    }

    protected virtual void PauseGame()
    {
        _bPaused = true;
        storedVelocity = body.velocity;
        body.velocity = Vector2.zero;
        body.isKinematic = true;
    }

    protected virtual void ResumeGame()
    {
        _bPaused = false;
        body.velocity = storedVelocity;
        body.isKinematic = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageCooldownTimer > 0) return;
        if (other.name == "HypersonicMask" && InDamageObjects(BossDamageObjects.Hypersonic))
        {
            TakeDamage();
            damageCooldownTimer = 1f;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Stalactite" && other.collider.gameObject.GetComponent<Stalactite>().IsFalling())
        {
            if (InDamageObjects(BossDamageObjects.Stalactite))
            {
                TakeDamage();
            }
            else
            {
                other.transform.GetComponent<Rigidbody2D>().velocity = Vector3.up;
                other.transform.Rotate(Vector3.forward, 5f);
            }
        }
    }

    protected void TakeDamage(int damage = 1)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine("Damaged");
        }
        bossProps.HealthChanged(health);
        HealthUpdate();
    }

    protected virtual void HealthUpdate() { }

    protected virtual void Die()
    {
        bossCollider.enabled = false;
        body.velocity = new Vector2(3f, 5f);
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

    private bool InDamageObjects(BossDamageObjects obj)
    {
        bool isInList = false;
        foreach(var damageObj in damageObjects)
        {
            if (damageObj == obj)
                isInList = true;
        }
        return isInList;
    }
    
    public void FaceDirection(bool bFaceLeft)
    {
        if (bFaceLeft)
            transform.localScale = new Vector2(originalScale.x, originalScale.y);
        else
            transform.localScale = new Vector2(-originalScale.x, originalScale.y);
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }
}
