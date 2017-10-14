using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenCrystalBall : MonoBehaviour {

    private Rigidbody2D[] pieces;
    private const int numPieces = 18;

	private void Start () {
        pieces = new Rigidbody2D[numPieces];
        int i = 0;
        foreach (Transform tf in transform)
        {
            pieces[i] = tf.GetComponent<Rigidbody2D>();
            pieces[i].isKinematic = true;
            i++;
        }
        gameObject.SetActive(false);
    }

    public void Shatter(Vector2 collisionPoint)
    {
        gameObject.SetActive(true);

        Vector2 velocityLower = Vector2.zero;
        Vector2 velocityUpper = Vector2.zero;

        if (transform.position.x > collisionPoint.x + 0.2f)
        {
            velocityLower.x = -2f;
            velocityUpper.x = 6f;
        }
        else if (transform.position.x < collisionPoint.x - 0.2f)
        {
            velocityLower.x = -6f;
            velocityUpper.x = 2f;
        }
        else
        {
            velocityLower.x = -3f;
            velocityUpper.x = 3f;
        }

        if (transform.position.y > collisionPoint.y + 0.2f)
        {
            velocityLower.y = -3f;
            velocityUpper.y = 5f;
        }
        else if (transform.position.y < collisionPoint.y - 0.2f)
        {
            velocityLower.y = -6f;
            velocityUpper.y = 2f;
        }
        else
        {
            velocityLower.y = -4f;
            velocityUpper.y = 3f;
        }
        
        for (int i = 0; i < numPieces; i++)
        {
            float xVel = Random.Range(velocityLower.x, velocityUpper.x);
            float yVel = Random.Range(velocityLower.y, velocityUpper.y);

            pieces[i].isKinematic = false;
            pieces[i].velocity = new Vector3(xVel, yVel, 0f);
        }
    }

    public void Shatter()
    {
        gameObject.SetActive(true);
        
        for (int i = 0; i < numPieces; i++)
        {
            float xVel = 0f;
            if (transform.position.x > Toolbox.PlayerCam.transform.position.x + 1f)
            {
                xVel = Random.Range(-2f, 10f);
            }
            else if (transform.position.x < Toolbox.PlayerCam.transform.position.x - 1f)
            {
                xVel = Random.Range(-10f, 2f);
            }
            else
            {
                xVel = Random.Range(-4f, 4f);
            }

            pieces[i].isKinematic = false;
            pieces[i].velocity = new Vector3(xVel, Random.Range(-4, 10f), 0f);
        }
    }

    public void ShatterAndDespawn(Vector2 collisionPoint)
    {
        Shatter(collisionPoint);
        StartCoroutine(DespawnRoutine());
    }

    public void ShatterAndDespawn()
    {
        Shatter();
        StartCoroutine(DespawnRoutine());
    }

    private IEnumerator DespawnRoutine()
    {
        float timer = 0f;
        const float duration = 4f;

        while (timer < duration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }

        for (int i = 0; i < numPieces; i++)
        {
            pieces[i].GetComponent<Collider2D>().enabled = false;
        }

        timer = 0f;
        while (timer < duration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }
        
        GetComponentInParent<MothCrystal>().DespawnToEarth();
    }
}
