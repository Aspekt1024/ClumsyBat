using System.Collections;
using UnityEngine;

public class JumpPound : BossAbility
{
    private enum JumpState
    {
        Idle,
        Falling,
        Jumping
    }
    private JumpState _state;
    private Rigidbody2D bossBody;
    private BaseAction callerAction;
    
    private void Start()
    {
        bossBody = GetComponent<Rigidbody2D>();
    }

    public void Jump(BaseAction caller, float jumpForce)
    {
        callerAction = caller;
        StartCoroutine("JumpAndPound", jumpForce);
    }

    private void Update()
    {
        if (_state == JumpState.Jumping && bossBody.velocity.y < 0)
        {
            _state = JumpState.Falling;
            
            if (callerAction.GetType().Equals(typeof(JumpAction)))
                ((JumpAction)callerAction).TopOfJump();
        }
    }

    private IEnumerator JumpAndPound(float jumpForce = 1000f)
    {
        bossBody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        bossBody.AddForce(Vector2.up * jumpForce);
        yield return null;
        _state = JumpState.Jumping;

        while (bossBody.velocity.y >= 0)
        {
            yield return null;
        }

        bossBody.AddForce(Vector2.down * jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_state != JumpState.Falling) return;
        if (other.collider.tag == "BossFloor")
        {
            _state = JumpState.Idle;
            BossEvents.JumpLanded();
            
            CameraEventListener.CameraShake(0.7f);
            bossBody.velocity = Vector2.zero;
            bossBody.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (callerAction.GetType().Equals(typeof(JumpAction)))
                ((JumpAction)callerAction).Landed();
        }
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
