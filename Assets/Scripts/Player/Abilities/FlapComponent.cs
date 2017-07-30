using UnityEngine;

public class FlapComponent : MonoBehaviour {

    private Player player;
    private Rigidbody2D playerBody;
    private const float horizontalVelocity = 4f;
    private const float verticalVelocity = 9f;

    public FlapComponent(Player player)
    {
        this.player = player;
    }

    public enum MovementMode
    {
        VerticalOnly,
        HorizontalEnabled
    }
    public MovementMode Mode;

    private enum HorizMoveState
    {
        None, MoveLeft, MoveRight
    }
    private HorizMoveState horizontalState;
    private const float moveDuration = 0.7f;
    private float moveTimer;

    private void Start()
    {
        player = Toolbox.Player;
        playerBody = player.GetBody();
    }

    private void Update()
    {
        if (horizontalState == HorizMoveState.None || Toolbox.Instance.GamePaused) return;
        moveTimer += Time.deltaTime;
        if (moveTimer > moveDuration)
        {
            horizontalState = HorizMoveState.None;
        }
        else
        {
            float velocityX = horizontalState == HorizMoveState.MoveLeft ? -horizontalVelocity : horizontalVelocity;
            playerBody.velocity = new Vector2(velocityX, playerBody.velocity.y);
        }
    }
    
    public void Flap(InputManager.TapDirection tapDir = InputManager.TapDirection.Center)
    {
        if (Mode != MovementMode.VerticalOnly)
        {
            SetHorizontalState(tapDir);
            moveTimer = 0f;
        }
        else
        {
            player.SetPlayerSpeed(Toolbox.Instance.PlayerSpeed);
        }
        var velocity = new Vector2(0f, verticalVelocity);
        player.SetVelocity(velocity);
        
        GameData.Instance.Data.Stats.TotalJumps++;
        player.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Flap);
        player.PlaySound(ClumsyAudioControl.PlayerSounds.Flap);
    }

    private void SetHorizontalState(InputManager.TapDirection tapDir)
    {
        if (tapDir == InputManager.TapDirection.Left)
        {
            horizontalState = HorizMoveState.MoveLeft;
            Faceleft();
        }
        else if (tapDir == InputManager.TapDirection.Right)
        {
            horizontalState = HorizMoveState.MoveRight;
            FaceRight();
        }
        else
            horizontalState = HorizMoveState.None;
    }
    
    public void Faceleft()
    {
        Vector3 scale = Toolbox.Player.transform.localScale;
        if (scale.x > 0)
        {
            Toolbox.Player.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            Toolbox.Player.transform.position += Vector3.right * .5f;
            Toolbox.Player.Lantern.GetComponent<HingeJoint2D>().limits = new JointAngleLimits2D()
            {
                min = -260f, max = -220f
            };
        }

    }

    public void FaceRight()
    {
        Vector3 scale = Toolbox.Player.transform.localScale;
        if (scale.x < 0)
        {
            Toolbox.Player.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            Toolbox.Player.transform.position += Vector3.left * .5f;
            Toolbox.Player.Lantern.GetComponent<HingeJoint2D>().limits = new JointAngleLimits2D()
            {
                min = -20f, max = 40f
            };
        }
    }

    public bool IsFacingRight()
    {
        return Toolbox.Player.transform.localScale.x > 0;
    }

    public void CancelIfMoving()
    {
        horizontalState = HorizMoveState.None;
    }
}
