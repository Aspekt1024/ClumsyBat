using ClumsyBat.Players;
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
    
    // For pausing/unpausing
    private Vector2 storedVelocity;
    private float storedAngularVelocity;
    private bool kinematicState;

    private ProjectileAbility projectileAbility;
    
    private void Awake()
    {
        bActive = true;
        projectileBody = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
    }

    public virtual void Activate() { }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!bActive) return;
        if (collision.gameObject.tag == "Player")
        {
            callerAction.HitPlayer();
            PlayerCollision();
        }
        else if (collision.gameObject.tag == "BossFloor" && transform.position.y < 0)
        {
            callerAction.Landed(this);
            projectileCollider.enabled = false;
            projectileBody.AddForce(new Vector2(0f, 200f));
        }
        else if (collision.gameObject.tag == "Stalactite")
        {
            StalactiteCollision(collision.collider);
        }

        if (collision.gameObject.tag.Contains("Cave") || collision.gameObject.tag.Equals("BossFloor"))
        {
            CaveCollision(collision.gameObject.tag);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Hypersonic")
        {
            HypersonicCollision(other.transform.position);
        }
    }

    public virtual void Pause()
    {
        kinematicState = projectileBody.isKinematic;
        storedVelocity = projectileBody.velocity;
        storedAngularVelocity = projectileBody.angularVelocity;

        projectileBody.velocity = Vector2.zero;
        projectileBody.angularVelocity = 0f;
        projectileBody.isKinematic = true;
    }

    public virtual void Resume()
    {
        projectileBody.isKinematic = kinematicState;
        if (!projectileBody.isKinematic)
        {
            projectileBody.velocity = storedVelocity;
            projectileBody.angularVelocity = storedAngularVelocity;
        }
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
    public void DespawnToEarth(float delay = 0f)
    {
        if (!bActive) return;
        bActive = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine(ToEarthAnimation(delay));
    }
    
    protected virtual void CaveCollision(string objectTag) { }
    protected virtual void PlayerCollision() { }
    protected virtual void StalactiteCollision(Collider2D stalactite) { }
    protected virtual void HypersonicCollision(Vector3 hypersonicOrigin) { }

    private IEnumerator ToEarthAnimation(float delay)
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

        animTimer = 0f;
        while (animTimer < delay)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
            }
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
