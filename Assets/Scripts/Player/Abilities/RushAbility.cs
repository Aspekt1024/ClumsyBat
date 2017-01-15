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
    private bool bDisabled = false;
    private float RushTimeRemaining = 0f;
    private float CooldownRemaining = 0f;

    private const float RushDuration = 0.26f;
    private const float RushSpeed = 7f;
    private const float NormalSpeed = 1f;
    private const float CooldownDuration = 1.5f;

    private bool bPaused = false;
    private bool bAtCaveEnd = false;

    StatsHandler Stats = null;
    Player ThePlayer = null;
    Rigidbody2D PlayerBody = null;
    Lantern Lantern = null;
    
    public void Setup(StatsHandler StatsRef, Player PlayerRef, Lantern LanternRef)
    {
        Stats = StatsRef;
        RushStats = Stats.AbilityData.GetRushStats();

        ThePlayer = PlayerRef;
        Lantern = LanternRef;
        PlayerBody = ThePlayer.GetComponent<Rigidbody2D>();

        SetAbilityAttributes();
        SetupHUDBar();
    }

    void Update ()
    {
        if (bPaused) { return; }

        CooldownRemaining -= Time.deltaTime;
        RushTimeRemaining -= Time.deltaTime;
        ThePlayer.Level.GameHUD.SetCooldown(1f - Mathf.Clamp(CooldownRemaining / CooldownDuration, 0f, 1f));

        if (bDisabled)
        {
            RushTimeRemaining = 0f;
            bIsRushing = false;
        }

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
        // TODO charges

        bDisabled = false;
        Stats.TimesDashed++;
        CooldownRemaining = CooldownDuration;
        StartCoroutine("RushStartAnimation");
    }

    public void Deactivate()
    {
        bDisabled = true;   // Sets the flag for the coroutine to deactivate
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
        ThePlayer.Level.GameHUD.SetCooldown(NumMoths / NumMothsToRecharge);
    }

    private void SetupHUDBar()
    {
        if (RushStats.AbilityAvailable)
        {
            ThePlayer.Level.GameHUD.ShowCooldown(true);
        }
        else
        {
            ThePlayer.Level.GameHUD.ShowCooldown(false);
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
        ThePlayer.SetGravity(0f);
        ThePlayer.SetVelocity(new Vector2(8f, 0f));
        if (EpicDashEnabled)
        {
            ThePlayer.SetAnimation("Rush"); // TODO special dash
        }
        else
        {
            ThePlayer.SetAnimation("Rush");
        }
        Lantern.AddRushForce();

        const float StartupDuration = 0.07f;
        float AnimTimer = 0f;
        while (AnimTimer < StartupDuration && !bDisabled)
        {
            if (!bPaused)
            {
                AnimTimer += Time.deltaTime;
            }
            yield return null;
        }

        if (!bPaused)
        {
            ThePlayer.SetPlayerSpeed(RushSpeed);
            ThePlayer.SetVelocity(Vector2.zero);
        }
    }

    private IEnumerator RushEndAnimation()
    {
        ThePlayer.SetPlayerSpeed(NormalSpeed);
        bIsRushing = false;
        ThePlayer.SetGravity(-1f);  // -1 resets to default gravity defined by PlayerController

        while (PlayerBody.position.x > Toolbox.PlayerStartX && !bAtCaveEnd && !bDisabled)
        {
            if (!bPaused)
            {
                float NewSpeed = 3 - (2 * PlayerBody.position.x / (PlayerBody.position.x + Toolbox.PlayerStartX));
                ThePlayer.SetVelocity(new Vector2(-NewSpeed, PlayerBody.velocity.y));
            }
            yield return null;
        }
        if (!bDisabled)
        {
            ThePlayer.SetVelocity(new Vector2(0, PlayerBody.velocity.y));
            ThePlayer.SetAnimation("Flap");
        }
    }

    public void CaveEndReached()
    {
        bAtCaveEnd = true;
    }

    public bool IsActive()
    {
        return !bDisabled;
    }
}
