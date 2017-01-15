using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    private int ShieldCharges = 0;
    private int MaxCharges = 1;
    private bool bPaused;

    private Player ThePlayer;
    private Lantern Lantern;
    private Rigidbody2D PlayerBody;
    private Animator PlayerAnim;
    //private StatsHandler Stats;

    private enum ShieldStates
    {
        Idle,
        Disabled,
        Activated
    }
    private ShieldStates State = ShieldStates.Idle;
    
    public void ConsumeCharge()
    {
        if (State == ShieldStates.Idle && ShieldCharges > 0)
        {
            ShieldCharges--;
            StartCoroutine("ShieldUp");
        }
        else if (PlayerBody.position.y < 0f)
        {
            PlayerBody.velocity = new Vector2(PlayerBody.velocity.x, 4f);
        }
    }

    private IEnumerator ShieldUp()
    {
        State = ShieldStates.Activated;
        ThePlayer.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 1f);
        // TODO animations
        // PlayerAnim.Play("Knockback", 0, 0f);
        yield return StartCoroutine("Knockback");

        PlayerAnim.Play("Flap", 0, 0f);
        yield return StartCoroutine("MoveForward");

        State = ShieldStates.Idle;
        ThePlayer.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
    }

    private IEnumerator Knockback()
    {
        float KnockbackTimer = 0f;
        const float KnockbackDuration = 0.4f;

        if (!ThePlayer.AtCaveEnd()) { PlayerBody.velocity = new Vector2(-8f, PlayerBody.velocity.y); }
        
        while (KnockbackTimer < KnockbackDuration)
        {
            if (!bPaused)
            {
                float DisplacementX = PlayerBody.position.x - ThePlayer.GetHomePositionX();
                if (DisplacementX < -1f)
                {
                    PlayerBody.position -= new Vector2(DisplacementX + 1f, 0f);
                }

                if (!ThePlayer.AtCaveEnd())
                {
                    if (DisplacementX > 0f)
                    {
                        PlayerBody.position -= new Vector2(DisplacementX, 0f);
                    }
                    ThePlayer.Level.UpdateGameSpeed(DisplacementX * 1.6f);
                }
                KnockbackTimer += Time.deltaTime;
            }
            yield return null;
        }
    }

    private IEnumerator MoveForward()
    {
        float MoveForwardTimer = 0f;
        const float MoveForwardDuration = 0.4f;
        float PlayerStartX = PlayerBody.position.x;
        float PlayerEndX = ThePlayer.GetHomePositionX();
        const float GameSpeedStart = 0f;
        const float GameSpeedEnd = 1f;

        while (MoveForwardTimer < MoveForwardDuration && !ThePlayer.AtCaveEnd())
        {
            if (!bPaused && ThePlayer.IsAlive())
            {
                MoveForwardTimer += Time.deltaTime;
                PlayerBody.position = new Vector2(PlayerStartX - (PlayerStartX - PlayerEndX) * (MoveForwardTimer / MoveForwardDuration), PlayerBody.position.y);
                ThePlayer.Level.UpdateGameSpeed(GameSpeedStart - (GameSpeedStart - GameSpeedEnd) * (MoveForwardTimer / MoveForwardDuration));
            }
            yield return null;
        }
        ThePlayer.Level.UpdateGameSpeed(1f);
    }

    public void Setup(StatsHandler StatsRef, Player PlayerRef, Lantern LanternRef)
    {
        //Stats = StatsRef;
        //ShieldStats = Stats.AbilityData.GetShieldStats();
        //SetAbilityAttributes();

        ThePlayer = PlayerRef;
        Lantern = LanternRef;
        PlayerBody = ThePlayer.GetComponent<Rigidbody2D>();
        PlayerAnim = ThePlayer.GetComponent<Animator>();

    }

    public void GamePaused(bool bGamePaused) { bPaused = bGamePaused; }
    public bool IsAvailable() { return (ShieldCharges > 0 || State == ShieldStates.Activated); }
    public void AddCharge() { if (ShieldCharges < MaxCharges) { ShieldCharges++; } }

}
