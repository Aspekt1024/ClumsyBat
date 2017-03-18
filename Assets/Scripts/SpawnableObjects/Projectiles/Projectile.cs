using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private ProjectileAbility poolOwner;
    private ProjectileAction callerAction;
    private bool bActive;

	private void OnCollisionEnter2D(Collision2D other)
    {
        if (!bActive) return;
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().HitByObject();
            callerAction.HitPlayer();
        }
        else if (other.gameObject.tag == "BossFloor" && transform.position.y < 0)
        {
            callerAction.Landed(this);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 200f));
        }
    }

    public void SetPoolOwner(ProjectileAbility owner)
    {
        poolOwner = owner;
    }

    public void SetCaller(ProjectileAction caller)
    {
        bActive = true;
        callerAction = caller;
    }

    public void DespawnToEarth()
    {
        if (!bActive) return;
        bActive = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine("ToEarthAnimation");
    }

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
        DeactivateProjectile(originalScale);
    }


    private void DeactivateProjectile(Vector3 originalScale)
    {
        var projectileBody = GetComponent<Rigidbody2D>();
        transform.localScale = originalScale;
        projectileBody.velocity = Vector2.zero;
        projectileBody.position = Toolbox.Instance.HoldingArea;
    }

}
