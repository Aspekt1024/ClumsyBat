using UnityEngine;
using System.Collections;

public class RushAbility : MonoBehaviour {

    private AbilityContainer.AbilityType RushStats;

    private int NumMothsToRecharge;
    private bool EpicDashEnabled;
    private int MaxCharges;

    private int NumCharges = 0;
    private int NumMoths = 0;
    private bool bIsRushing = false;
    private float RushTimeRemaining = 0f;
    private float CooldownRemaining = 0f;

    private const float RushDuration = 0.26f;
    private const float RushSpeed = 7f;
    private const float NormalSpeed = 1f;
    private const float CooldownDuration = 1.5f;

    private bool bPaused = false;
    private bool bAtCaveEnd = false;

    StatsHandler Stats = null;
    PlayerController PlayerControl = null;
    Rigidbody2D PlayerBody = null;
    Lantern Lantern = null;
    
    public void Setup(StatsHandler StatsRef, PlayerController PlayerRef, Lantern LanternRef)
    {
        Stats = StatsRef;
        RushStats = Stats.AbilityData.GetRushStats();

        PlayerControl = PlayerRef;
        Lantern = LanternRef;
        PlayerBody = PlayerControl.GetComponent<Rigidbody2D>();

        SetAbilityAttributes();
        SetupHUDBar();
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
        if (!RushStats.AbilityAvailable) { return; }
        if (CooldownRemaining > 0) { return; }

        Stats.TimesDashed++;
        CooldownRemaining = CooldownDuration;
        StartCoroutine("RushStartAnimation");
    }

    private void SetAbilityAttributes()
    {
        NumMoths = 0;
        NumMothsToRecharge = 3;
        MaxCharges = 1;
        EpicDashEnabled = false;

        if (RushStats.AbilityLevel >= 2) { NumMothsToRecharge = 2; }
        if (RushStats.AbilityLevel >= 3) { MaxCharges = 2; }
        if (RushStats.AbilityLevel >= 4) { NumMothsToRecharge = 1; }
        if (RushStats.AbilityLevel >= 5) { MaxCharges = 3; }
        if (RushStats.AbilityEvolution == 2) { EpicDashEnabled = true; }
    }

    public void MothConsumed()
    {
        NumMoths++;
        if (NumMoths == NumMothsToRecharge)
        {
            NumCharges = Mathf.Clamp(NumCharges++, 0, MaxCharges);
            NumMoths = (NumCharges == MaxCharges ? 1 : 0);
        }
        PlayerControl.Level.GameHUD.SetCooldown(NumMoths / NumMothsToRecharge);
    }

    private void SetupHUDBar()
    {
        if (RushStats.AbilityAvailable)
        {
            PlayerControl.Level.GameHUD.ShowCooldown(true);
        }
        else
        {
            PlayerControl.Level.GameHUD.ShowCooldown(false);
        }
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
        if (EpicDashEnabled)
        {
            PlayerControl.SetAnimation("Rush"); // TODO special dash
        }
        else
        {
            PlayerControl.SetAnimation("Rush");
        }
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
