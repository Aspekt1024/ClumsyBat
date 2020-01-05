using System.Collections;
using ClumsyBat;
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

    private Coroutine jumpRoutine;
    
    private void Start()
    {
        bossBody = GetComponent<Rigidbody2D>();
    }

    public void Jump(BaseAction caller, float jumpForce)
    {
        callerAction = caller;
        if (jumpRoutine != null)
        {
            StopCoroutine(jumpRoutine);
        }
        jumpRoutine = StartCoroutine(JumpAndPound(jumpForce));
    }

    private void Update()
    {
        if (_state == JumpState.Jumping && bossBody.velocity.y < 0)
        {
            _state = JumpState.Falling;
            
            if (callerAction.GetType() == typeof(JumpAction))
                ((JumpAction)callerAction).TopOfJump();
        }
    }

    private IEnumerator JumpAndPound(float jumpForce = 1000f)
    {
        bossBody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        bossBody.velocity = new Vector2(bossBody.velocity.x, jumpForce / 50f);
        //bossBody.AddForce(Vector2.up * jumpForce);
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
        if (_state != JumpState.Falling || !other.collider.CompareTag("BossFloor")) return;
        
        _state = JumpState.Idle;
        BossEvents.JumpLanded();
            
        GameStatics.Camera.Shake(0.7f);
        bossBody.velocity = Vector2.zero;
        bossBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            
        GameStatics.Audio.Boss.PlaySound(BossSounds.BossStomp);

        if (callerAction.GetType() == typeof(JumpAction))
        {
            ((JumpAction)callerAction).Landed();
        }
    }
}
