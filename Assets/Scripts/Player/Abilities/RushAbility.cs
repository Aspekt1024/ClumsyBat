using UnityEngine;
using System.Collections;

public class RushAbility : MonoBehaviour {

    private bool bPaused = false;
    private bool bEnabled = false;
    private int AbilityLevel = 1;

    private float CooldownDuration = 5f;        // TODO calculate this from level
    private float CooldownRemaining = 0f;

    private bool bIsRushing = false;
    private float RushTimeRemaining = 0f;
    private const float RushDuration = 0.26f;
    private const float RushSpeed = 7f;
    private const float NormalSpeed = 1f;

    private bool bAtCaveEnd = false;

    StatsHandler Stats = null;
    PlayerController PlayerControl = null;
    Lantern Lantern = null;

    Rigidbody2D PlayerBody = null;

    public void Setup(StatsHandler StatsRef, PlayerController PlayerRef, Lantern LanternRef)
    {
        Stats = StatsRef;
        bEnabled = Stats.AbilityData.GetRushStats().AbilityUnlocked;
        AbilityLevel = Stats.AbilityData.GetRushStats().AbilityLevel;

        PlayerControl = PlayerRef;
        Lantern = LanternRef;

        PlayerBody = PlayerControl.GetComponent<Rigidbody2D>();
        
    }

    void Update ()
    {
        if (bPaused) { return; }

        CooldownRemaining -= Time.deltaTime;
        RushTimeRemaining -= Time.deltaTime;
        PlayerControl.Level.GameHUD.SetCooldown(1f - Mathf.Clamp(CooldownRemaining / CooldownDuration, 0f, 1f));

        if (bIsRushing)
        {
            Stats.DashDistance += Time.deltaTime * RushSpeed * Toolbox.Instance.LevelSpeed;
            if (RushTimeRemaining <= 0f)
            {
                StartCoroutine("RushEndAnimation");
            }
        }
    }

    public void Activate()
    {
        //if (!bEnabled) { return; }    // TODO figure out where to get this ability
        if (CooldownRemaining > 0)
        {
            // TODO rush fail animation
            return;
        }

        Stats.TimesDashed++;
        CooldownRemaining = CooldownDuration;
        StartCoroutine("RushStartAnimation");
    }

    public void GamePaused(bool _paused)
    {
        bPaused = _paused;
    }
    
    private IEnumerator RushStartAnimation()
    {
        bIsRushing = true;
        RushTimeRemaining = RushDuration;
        PlayerControl.SetGravity(0f);
        PlayerControl.SetVelocity(new Vector2(8f, 0f));
        PlayerControl.SetAnimation("Rush");
        Lantern.AddRushForce();

        const float StartupDuration = 0.07f;
        float AnimTimer = 0f;
        while (AnimTimer < StartupDuration)
        {
            if (!bPaused)
            {
                AnimTimer += Time.deltaTime;
            }
            yield return null;
        }

        if (!bPaused)
        {
            PlayerControl.SetPlayerSpeed(RushSpeed);
            PlayerControl.SetVelocity(Vector2.zero);
        }
    }

    private IEnumerator RushEndAnimation()
    {
        PlayerControl.SetPlayerSpeed(NormalSpeed);
        bIsRushing = false;
        PlayerControl.SetGravity(-1f);  // -1 resets to default gravity defined by PlayerController

        while (PlayerBody.position.x > Toolbox.PlayerStartX && !bAtCaveEnd)
        {
            if (!bPaused)
            {
                float NewSpeed = 3 - (2 * PlayerBody.position.x / (PlayerBody.position.x + Toolbox.PlayerStartX));
                PlayerControl.SetVelocity(new Vector2(-NewSpeed, PlayerBody.velocity.y));
            }
            yield return null;
        }
        PlayerControl.SetVelocity(new Vector2(0, PlayerBody.velocity.y));
        PlayerControl.SetAnimation("Flap");
    }

    public void CaveEndReached()
    {
        bAtCaveEnd = true;
    }
}
