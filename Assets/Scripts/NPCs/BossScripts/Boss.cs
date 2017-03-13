using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using BossDamageObjects = MachineState.BossDamageObjects;

public class Boss : MonoBehaviour {
    
    protected bool _bPaused;

    private int health;
    private SpriteRenderer bossRenderer;
    private Rigidbody2D body;
    private Collider2D bossCollider;

    private Vector2 storedVelocity;
    
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
        body = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<Collider2D>();
        bossRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBaseProperties(BossCreator props)
    {
        health = props.Health;
    }

    public void SetPropsFromState(BossState state)
    {
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

    public void HitByHypersonic()
    {
        if (InDamageObjects(BossDamageObjects.Hypersonic))
        {
            TakeDamage();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Stalactite" && InDamageObjects(BossDamageObjects.Stalactite))
            TakeDamage();
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
    }

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

        Color originalColor = bossRenderer.color;

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
        bossRenderer.color = originalColor;
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
}
