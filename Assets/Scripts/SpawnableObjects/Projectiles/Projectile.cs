using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a boss ability weapon.
/// This could also be used for standard level mechanics if decided later.
/// </summary>
public class Projectile : MonoBehaviour {

    public Rigidbody2D Body
    {
        get { return projectileBody; }
    }
    public Collider2D Collider
    {
        get { return projectileCollider; }
    }
    
    protected ProjectileAction callerAction;
    protected bool bActive;

    protected Rigidbody2D projectileBody;
    protected Collider2D projectileCollider;

    protected Vector2 storedVelocity;
    protected float storedAngularVelocity;

    private ProjectileAbility projectileAbility;
    
    private void Awake()
    {
        bActive = true;
        projectileBody = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
        Activated();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!bActive) return;
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().HitByObject();
            PlayerCollision();
            callerAction.HitPlayer();
        }
        else if (other.gameObject.tag == "BossFloor" && transform.position.y < 0)
        {
            callerAction.Landed(this);
            projectileCollider.enabled = false;
            projectileBody.AddForce(new Vector2(0f, 200f));
        }

        if (other.gameObject.tag.Contains("Cave") || other.gameObject.tag.Equals("BossFloor"))
        {
            CaveCollision();
        }
    }

    public virtual void Pause()
    {
        storedVelocity = projectileBody.velocity;
        storedAngularVelocity = projectileBody.angularVelocity;
        projectileBody.velocity = Vector2.zero;
        projectileBody.angularVelocity = 0f;
        projectileBody.isKinematic = true;
    }

    public virtual void Resume()
    {
        projectileBody.isKinematic = false;
        projectileBody.velocity = storedVelocity;
        projectileBody.angularVelocity = storedAngularVelocity;
    }

    public void SetReferences(ProjectileAction caller, ProjectileAbility ability)
    {
        callerAction = caller;
        projectileAbility = ability;
    }

    /// <summary>
    /// Called by another node in the Boss State Machine
    /// E.g. Spawning a stalacmite from a rock
    /// </summary>
    public void DespawnToEarth()
    {
        if (!bActive) return;
        bActive = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine(ToEarthAnimation());
    }

    protected virtual void Activated() { }
    protected virtual void CaveCollision() { }
    protected virtual void PlayerCollision() { }

    private IEnumerator ToEarthAnimation()
    {
        float animTimer = 0f;
        const float animDuration = 1f;

        Vector3 originalScale = transform.localScale;

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            transform.localScale *= (1 - animTimer / animDuration);
            transform.position += Vector3.down * 0.1f;
            yield return null;
        }
        DeactivateProjectile();
    }
    
    private void DeactivateProjectile()
    {
        if (projectileAbility != null)
        {
            projectileAbility.RemoveProjectile(this);
        }
        Destroy(gameObject);
    }
    
    public bool IsType<T>() { return GetType().Equals(typeof(T)); }
    public bool IsRock() { return GetType().Equals(typeof(Rock)); }
    public bool IsMothCrystal() { return GetType().Equals(typeof(MothCrystal)); }
}
