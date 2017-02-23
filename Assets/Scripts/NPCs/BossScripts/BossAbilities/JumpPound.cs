using System.Collections;
using UnityEngine;

public class JumpPound : BossAbility
{
    private enum JumpState
    {
        Idle,
        Jumping
    }
    private JumpState _state;
    
    public override void Activate()
    {
        _state = JumpState.Jumping;
        StartCoroutine("JumpAndPound");
    }

    private IEnumerator JumpAndPound()
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1000f);
        
        yield return new WaitForSeconds(0.5f);

        while (GetComponent<Rigidbody2D>().velocity.y >= 0)
        {
            yield return null;
        }

        GetComponent<Rigidbody2D>().AddForce(Vector2.down * 1000f);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_state != JumpState.Jumping) return;
        if (other.collider.tag == "BossFloor")
        {
            StartCoroutine("ShakeScreen", 0.5f);
            _state = JumpState.Idle;
            BossEvents.JumpLanded();
        }
    }

    private IEnumerator ShakeScreen(float time)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.up * 0.5f;    // Prevents the boss from falling through the floor
        CameraEventListener.CameraShake();
        yield return StartCoroutine("WaitSeconds", time);
        CameraEventListener.StopCameraShake();
    }
    
    private IEnumerator WaitSeconds(float secs)
    {
        float timer = 0f;
        while (timer < secs)
        {
            if (!Toolbox.Instance.GamePaused)
                timer += Time.deltaTime;
            yield return null;
        }
    }

}
