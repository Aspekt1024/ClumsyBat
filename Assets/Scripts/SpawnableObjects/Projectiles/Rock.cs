using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Projectile {

    // Set in inspector
    public GameObject RockShatterObject;

    private ParticleSystem rockParticleSystem;
    private bool particleSystemIsPlaying;

    private float timer;
    private const float timeBeforeColliderEnabled = 0.12f;
    
    public override void Pause()
    {
        base.Pause();
        if (particleSystemIsPlaying)
        {
            rockParticleSystem.Pause();
        }
    }

    public override void Resume()
    {
        base.Resume();
        if (particleSystemIsPlaying)
        {
            rockParticleSystem.Play();
        }
    }

    public override void Activate()
    {
        rockParticleSystem = RockShatterObject.GetComponent<ParticleSystem>();
        rockParticleSystem.Stop();
        timer = 0;
        Collider.enabled = false;
    }

    private void Update()
    {
        if (Collider.enabled || Toolbox.Instance.GamePaused) return;
        
        timer += Time.deltaTime;
        if (timer >= timeBeforeColliderEnabled)
        {
            Collider.enabled = true;
        }
    }

    protected override void CaveCollision(string objectTag)
    {
        if (objectTag.Contains("Wall"))
        {
            float horizontalForce = Random.Range(100f, 250f);

            if (transform.position.x < Camera.main.transform.position.x)
            {
                projectileBody.AddForce(new Vector2(horizontalForce, 10f));
            }
            else
            {
                projectileBody.AddForce(new Vector2(-horizontalForce, 10f));
            }
        }
    }

    protected override void PlayerCollision()
    {
        Shatter();
    }

    protected override void HypersonicCollision(Vector3 hypersonicOrigin)
    {
        Shatter();
    }

    protected override void StalactiteCollision(Collider2D stalCollider)
    {
        Stalactite stalScript = stalCollider.GetComponent<Stalactite>();
        if (stalScript == null)
        {
            stalScript = stalCollider.GetComponentInParent<Stalactite>();
        }
        stalScript.Crack();
        Shatter();
    }
    
    private void Shatter()
    {
        particleSystemIsPlaying = true;
        projectileCollider.enabled = false;
        projectileBody.velocity = Vector2.zero;
        projectileBody.angularVelocity = 0f;
        projectileBody.isKinematic = true;

        rockParticleSystem.Play();

        DespawnToEarth(3f);
    }
}
