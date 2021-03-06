﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClumsyBat.Objects;
using ClumsyBat;

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
    public Transform ProjectilePoint;

    protected bool _bPaused;
    protected int health;
    protected SpriteRenderer bossRenderer;
    protected Collider2D bossCollider;

    private StateMachine machine;

    private Color originalColor;
    private Vector2 storedVelocity;
    private float damageCooldownTimer;
    
    private void Awake()
    {
        GetBossComponents();
        if (bossRenderer != null)
        {
            originalColor = bossRenderer.color;
        }
    }

    private void Update()
    {
        BossUpdate();

        if (damageCooldownTimer < 0) return;
        damageCooldownTimer -= Time.deltaTime;
    }

    protected virtual void BossUpdate() { }
    protected abstract void GetBossComponents();
    protected abstract void DeathSequence();

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
        if (Body == null) return;   // Not all bosses need a body (e.g. Hypersonic event boss)
        storedVelocity = Body.velocity;
        Body.velocity = Vector2.zero;
        Body.isKinematic = true;
    }

    protected virtual void ResumeGame()
    {
        _bPaused = false;
        if (Body == null) return;
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
            if (other.collider.gameObject.GetComponent<Stalactite>().IsFalling)
                machine.Damaged(CollisionAction.CollisionTypes.FallingStalactite, other.collider);
            else
                machine.Damaged(CollisionAction.CollisionTypes.StaticStalactite, other.collider);
        }
        else if (other.collider.tag == "Player")
        {
            if (GameStatics.Player.Clumsy.State.IsRushing)
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

    public void StopCurrentActions()
    {
        machine.Stop();
    }


    protected virtual void HealthUpdate() { }

    protected virtual void Die()
    {
        machine.Stop();
        DeathSequence();
        BossEvents.BossDeath();
        GameStatics.Data.Stats.BossesDefeated++;
        GameStatics.Audio.Boss.PlaySound(BossSounds.BossDeath);
    }

    private IEnumerator Damaged()
    {
        const int numFlashes = 8;
        const float flashDuration = 0.05f;
        
        bool flashOn = true;
        GameStatics.Audio.Boss.PlaySound(BossSounds.BossDamaged);

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
        bossRenderer.color = originalColor;
    }

    public bool IsPaused()
    {
        return _bPaused;
    }

    public virtual void Walk() { }
    public virtual void EndWalk() { }
    public virtual void Jump() { }
    public virtual void EndJump() { }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }

    public Vector3 GetProjectilePoint()
    {
        if (ProjectilePoint != null)
        {
            return ProjectilePoint.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public bool IsDead { get { return health <= 0; } }
    public bool IsAlive { get { return health > 0; } }
}
