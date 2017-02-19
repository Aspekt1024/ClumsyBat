using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPound : MonoBehaviour
{
    private Vector3 _startPos;
    
    public void Activate()
    {
        _startPos = transform.position;
        StartCoroutine("JumpAndPound");
    }

    private IEnumerator JumpAndPound()
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1000f);
        
        yield return new WaitForSeconds(0.5f);

        while (GetComponent<Rigidbody2D>().velocity.y > 0)
        {
            yield return null;
        }

        GetComponent<Rigidbody2D>().AddForce(Vector2.down * 1000f);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "BossFloor")
        {
            StartCoroutine("ShakeScreen", 0.5f);
        }
    }

    private IEnumerator ShakeScreen(float time)
    {
        CameraEventListener.CameraShake();
        yield return new WaitForSeconds(time);
        CameraEventListener.StopCameraShake();
    }

}
