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

    public void Jump(BaseAction caller)
    {
        _state = JumpState.Jumping;
        callerAction = caller;
        StartCoroutine("JumpAndPound");
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
        if (_state != JumpState.Falling) return;
        if (other.collider.tag == "BossFloor")
        {
            _state = JumpState.Idle;
            BossEvents.JumpLanded();

            GetComponent<Rigidbody2D>().velocity = Vector2.up * 0.5f;    // Prevents the boss from falling through the floor // TODO replace collider with floor collider

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
