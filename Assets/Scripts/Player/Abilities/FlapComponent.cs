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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerBody = player.gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (horizontalState == HorizMoveState.None) return;
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
        var velocity = new Vector2(0f, verticalVelocity);
        player.SetVelocity(velocity);

        GameData.Instance.Data.Stats.TotalJumps++;
        player.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Flap);
        player.PlaySound(ClumsyAudioControl.PlayerSounds.Flap);
    }

    private void SetHorizontalState(InputManager.TapDirection tapDir)
    {
        if (tapDir == InputManager.TapDirection.Left)
            horizontalState = HorizMoveState.MoveLeft;
        else if (tapDir == InputManager.TapDirection.Right)
            horizontalState = HorizMoveState.MoveRight;
        else
            horizontalState = HorizMoveState.None;
    }

    public void CancelIfMoving()
    {
        horizontalState = HorizMoveState.None;
    }
}
