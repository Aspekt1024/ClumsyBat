using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalPiece : MonoBehaviour {

    private bool hitFloor;
    private bool hitByOther;
    private Rigidbody2D body;

    private bool paused;
    private Vector2 storedVelocity;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (paused == Toolbox.Instance.GamePaused) return;
        paused = Toolbox.Instance.GamePaused;

        if (paused)
        {
            storedVelocity = body.velocity;
            body.velocity = Vector2.zero;
            body.isKinematic = true;
        }
        else
        {
            body.isKinematic = false;
            body.velocity = storedVelocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (transform.position.y > 0) return;

        if (other.collider.tag == "BossFloor" && !hitFloor)
        {
            body.AddForce(Vector2.up * 200f);
            gameObject.layer = LayerMask.NameToLayer("Stalactites");
            StartCoroutine(DissolvePiece());
        }
        else if (other.collider.tag == "Boss" && !hitByOther)
        {
            Vector2 bossVelocity = other.collider.GetComponentInParent<Rigidbody2D>().velocity;
            float xForce = Random.Range(bossVelocity.x * 20f, bossVelocity.x * 50f);
            float yForce = Random.Range(Mathf.Abs(bossVelocity.x) * 50f, Mathf.Abs(bossVelocity.x) * 100f);

            Vector2 trajectoryForce = new Vector2(xForce, yForce);
            body.AddForce(trajectoryForce);

            gameObject.layer = LayerMask.NameToLayer("Rubble");
            StartCoroutine(DissolvePiece());
        }
    }

    private IEnumerator DissolvePiece()
    {
        hitByOther = true;
        hitFloor = true;

        const float activeDuration = 0.4f;
        const float persistentDuration = 1f;

        yield return StartCoroutine(Wait(activeDuration));
        gameObject.layer = LayerMask.NameToLayer("Rubble");
        yield return StartCoroutine(Wait(persistentDuration));

        float timer = 0f;
        const float animDuration = 0.3f;
        Vector2 originalScale = transform.localScale;
        while (timer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
                timer += Time.deltaTime;

            float sizeRatio = 1f - (timer / animDuration);
            transform.localScale = originalScale * sizeRatio;

            yield return null;
        }
    }

    private IEnumerator Wait(float waitTime)
    {
        float timer = 0f;
        while (timer < waitTime)
        {
            if (!Toolbox.Instance.GamePaused)
                timer += Time.deltaTime;

            yield return null;
        }
    }

}
