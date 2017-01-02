using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public delegate void PlayerDeathHandler(object sender, EventArgs e);

/// <summary>
/// Handles the player input (touch screen)
/// Determines how the player jumps and what happens when they hit an object
/// </summary>
public class PlayerController : MonoBehaviour
{
    public FogEffect Fog;
    public LevelScript Level;
    public Hypersonic Hypersonic;
    // TODO Rush Component
    // TODO Jump Component?

    private GameObject InputObject;
    private SwipeManager InputManager;
    
    private Vector3 PlayerHoldingArea = new Vector3(-10f, -10f, 0f);
    private Rigidbody2D PlayerRigidBody;
    private Rigidbody2D LanternBody;
    private Lantern Lantern;
    private Animator Anim;
    private JumpClearance Clearance;

    private const float GameStartupTime = 1f;

    private bool bToolTipWait = false;
    private bool bGameStarted = false;
    private bool bGamePaused = false;
    private bool bGameResuming = false;
    private float ResumeTimerStart;
    private float ResumeTimer = 3f;
    private Vector2 SavedVelocity = Vector2.zero;

    private bool bIsAlive = true;
    private bool bGameOverOverlay = false;

    public Vector2 JumpForce = new Vector2(0, 700);
    private const float GravityScale = 3;
    private float PlayerSpeed;

    private RushAbility Rush;

    public event PlayerDeathHandler PlayerDeath; // not currently used. Kept for reference (events!)
    
    protected virtual void OnDeath(EventArgs e)
    {
        if (PlayerDeath != null)
        {
            PlayerDeath(this, e);
        }
    }
    
    void Start()
    {
        PlayerSpeed = 1f;
        PlayerRigidBody = GetComponent<Rigidbody2D>();
        PlayerRigidBody.isKinematic = true;
        PlayerRigidBody.gravityScale = GravityScale;
        Clearance = GameObject.Find("JumpClearance").GetComponent<JumpClearance>();

        Anim = GetComponent<Animator>();

        LanternBody = GameObject.Find("Lantern").GetComponent<Rigidbody2D>();
        Lantern = LanternBody.GetComponent<Lantern>();

        InputObject = new GameObject("Game Scripts");
        InputManager = InputObject.AddComponent<SwipeManager>();

        Level.Stats.CollectedCurrency = 0;

        transform.position = new Vector3(-Toolbox.TileSizeX / 2f, 0f, transform.position.z);
        SetupAbilities();
    }

    private void SetupAbilities()
    {
        GameObject AbilityScripts = new GameObject("Ability Scripts");
        Rush = AbilityScripts.AddComponent<RushAbility>();

        Rush.Setup(Level.Stats, this, Lantern);
    }

    public void LevelStart()
    {
        StartCoroutine("LevelStartCountdown");
    }
    
    void Update()
    {
        if (bGamePaused || bGameResuming || bGameOverOverlay) { return; }
        Clearance.transform.position = transform.position;

        if (bIsAlive)
        {
            if (bGameStarted && !bGameResuming)
            {
                Level.AddDistance(Time.deltaTime, PlayerSpeed);
                if (Level.AtCaveEnd())
                {
                    Rush.CaveEndReached();
                    transform.position += new Vector3(Time.deltaTime * PlayerSpeed * Toolbox.Instance.LevelSpeed, 0f, 0f);
                }
            }
            ProcessInput();
        }

        if (!bGameStarted) { return; }  // TODO see if we still need this

        CheckForDeath();

        if (transform.position.y < 1f && !Level.Stats.CompletionData.ToolTipComplete(CompletionDataControl.ToolTipID.SecondJump))
        {
            StartCoroutine("ToolTipPause", CompletionDataControl.ToolTipID.SecondJump);
        }
    }

    private void ProcessInput()
    {
        if (bToolTipWait || !bGameStarted) { return; }
        if (!Level.Stats.CompletionData.ToolTipComplete(CompletionDataControl.ToolTipID.SecondJump)) { return; }

        if (InputManager.SwipeRegistered())
        {
            ProcessSwipe();
        }
        if (InputManager.TapRegistered())
        {
            ProcessTap();
        }
        
        if (Input.GetKeyUp("w"))
        {
            ProcessTap();
        }
        else if (Input.GetKeyUp("a"))
        {
            ProcessSwipe();
        }
    }

    private void ProcessTap()
    {
        if (bGamePaused) { ResumeGame(); }
        Jump();
    }

    private void ProcessSwipe()
    {
        if (bGamePaused) { return; }
        Rush.Activate();
    }

    private void CheckForDeath()
    {
        // Die by being off-screen
        Vector2 ScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (ScreenPosition.y > Screen.height || ScreenPosition.y < 0)
        {
            if (bIsAlive)
            {
                Die();
                transform.position = PlayerHoldingArea;
            }
            else
            {
                StartCoroutine("GameOverSequence");
            }
        }
    }

    public void SetGravity(float gravity)
    {
        PlayerRigidBody.gravityScale = (gravity == -1 ? GravityScale : gravity);
    }
    public void SetVelocity(Vector2 velocity)
    {
        PlayerRigidBody.velocity = velocity;
    }
    public void SetAnimation(string AnimationName)
    {
        Anim.Play(AnimationName, 0, 0f);
    }
    public void SetPlayerSpeed(float Speed)
    {
        PlayerSpeed = Speed;
        Level.UpdateGameSpeed(PlayerSpeed);
    }

    private IEnumerator GameOverSequence()
    {
        bGameOverOverlay = true;
        yield return new WaitForSeconds(1f);
        Level.ShowGameoverMenu();
    }

    void Jump()
    {
        Level.Stats.TotalJumps++;
        PlayerRigidBody.velocity = Vector2.zero;
        PlayerRigidBody.AddForce(JumpForce);
        //GetComponent<AudioSource>().Play();
        Anim.Play("Flap", 0, 0.5f);
    }

    // Die by Collision
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!bGameStarted) { return; }
        if (other.collider.GetComponent<Stalactite>())
        {
            Level.Stats.ToothDeaths++;
            other.collider.GetComponent<Stalactite>().Crack();
        }
        else
        {
            Level.Stats.RockDeaths++;
        }
        Die();
    }

    void Die()
    {
        Level.Stats.Deaths += 1;
        OnDeath(EventArgs.Empty);
        bIsAlive = false;
        GetComponent<CircleCollider2D>().enabled = false;
        PlayerRigidBody.gravityScale = GravityScale;
        PlayerRigidBody.velocity = new Vector2(1, 0);
        Level.HorribleDeath();
        Rush.GamePaused(true);
        Lantern.Drop();

        Anim.Play("Die", 0, 0.25f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.name)
        {
            case "MothTrigger":
                StartCoroutine("ConsumeMoth", other);
                break;
            case "ExitTrigger":
                CircleCollider2D ClumsyCollider = GetComponent<CircleCollider2D>();
                ClumsyCollider.enabled = false;
                PlayerRigidBody.isKinematic = true;
                StartCoroutine("CaveExitAnimation");
                break;
            case "Spore":
                Fog.Minimise();
                break;
            case "MothSensor":
                if (!Level.Stats.CompletionData.ToolTipComplete(CompletionDataControl.ToolTipID.FirstMoth))
                {
                    StartCoroutine("ToolTipPause", CompletionDataControl.ToolTipID.FirstMoth);
                }
                break;
        }
    }

    private IEnumerator ConsumeMoth(Collider2D MothCollider)
    {
        Level.Stats.MothsEaten++;
        if (Level.Stats.MothsEaten > Level.Stats.MostMoths)
        {
            Level.Stats.MostMoths++;
        }
        Level.Stats.TotalMoths++;
        Moth MothScript = MothCollider.GetComponentInParent<Moth>();
        float AnimationWaitTime = MothScript.ConsumeMoth();
        float AnimTimer = 0f;
        while (AnimTimer < AnimationWaitTime)
        {
            if (!bGamePaused)
            {
                AnimTimer += Time.deltaTime;
            }
            yield return null;
        }

        int CurrencyValue = 0;
        switch (MothScript.Colour)
        {
            case Moth.MothColour.Green:
                CurrencyValue = 1;
                Lantern.ChangeColour(Lantern.LanternColour.Green);
                Fog.Echolocate();
                break;
            case Moth.MothColour.Gold:
                CurrencyValue = 2;
                Lantern.ChangeColour(Lantern.LanternColour.Gold);
                StartHypersonic();
                Fog.Echolocate();
                break;
            case Moth.MothColour.Blue:
                CurrencyValue = 3;
                Lantern.ChangeColour(Lantern.LanternColour.Blue);
                Fog.Echolocate();
                break;
        }
        Level.Stats.CollectedCurrency += CurrencyValue;
        Level.GameHUD.UpdateCurrency(Pulse: true);
    }

    void StartGame()
    {
        bGameStarted = true;
        PlayerRigidBody.WakeUp();
        PlayerRigidBody.isKinematic = false;
        Level.StartGame();
        Fog.Resume();
        Anim.enabled = true;
    }

    public void PauseButtonPressed()
    {
        PauseGame(ShowMenu: true);
    }

    public void PauseGame(bool ShowMenu = true)
    {
        bGamePaused = true;
        Level.PauseGame(ShowMenu);
        SavedVelocity = PlayerRigidBody.velocity;
        PlayerRigidBody.Sleep();
        Fog.Pause();
        Level.Stats.SaveStats();
        Anim.enabled = false;
        Lantern.GamePaused(true);
        AbilitiesPaused(true);
    }

    private void AbilitiesPaused(bool bPauseAbility)
    {
        Rush.GamePaused(bPauseAbility);
    }

    private void StartHypersonic()
    {
        Hypersonic.ActivateHypersonic();
        Level.DestroyOnScreenEvils();
    }

    public void ResumeGame()
    {
        if (bGameResuming) { return; }
        Level.Stats.SaveStats();
        bGameResuming = true;
        
        StartCoroutine("UpdateResumeTimer");
    }

    IEnumerator UpdateResumeTimer()
    {
        float WaitTime = Level.GameMenu.RaiseMenu();
        yield return new WaitForSeconds(WaitTime);
        ResumeTimerStart = Time.time;
        
        while (bIsAlive && ResumeTimerStart + ResumeTimer - WaitTime > Time.time)
        {
            float TimeRemaining = ResumeTimerStart + ResumeTimer - Time.time;
            Level.GameHUD.SetResumeTimer(TimeRemaining);
            yield return null;
        }
        ResumeGameplay();
    }

    void ResumeGameplay()
    {
        bGamePaused = false;
        bGameResuming = false;
        InputManager.ClearInput();
        Level.GameHUD.HideResumeTimer();
        PlayerRigidBody.WakeUp();
        PlayerRigidBody.velocity = SavedVelocity;
        Level.ResumeGame();
        Fog.Resume();
        Anim.enabled = true;
        Lantern.GamePaused(false);

        if (bIsAlive)
        {
            Rush.GamePaused(false);
        }
    }

    IEnumerator CaveExitAnimation()
    {
        Vector2 TargetExitPoint = new Vector2(Toolbox.TileSizeX / 2f, 0f);
        while (transform.position.x < TargetExitPoint.x)
        {
            float XShift = Time.deltaTime * Toolbox.Instance.LevelSpeed;
            float YShift = (TargetExitPoint.y - transform.position.y) / (4 * Toolbox.Instance.LevelSpeed);
            transform.position += new Vector3(XShift, YShift, 0f);
            yield return null;
        }
        transform.position = PlayerHoldingArea;
        LanternBody.transform.position = new Vector3(LanternBody.transform.position.x + .3f, LanternBody.transform.position.y, LanternBody.transform.position.z);
        Level.LevelWon();
        Fog.EndOfLevel();
        bGamePaused = true;
    }

    IEnumerator CaveEntranceAnimation()
    {
        Vector3 StartPoint = transform.position;
        Vector2 TargetPoint = new Vector2(-5f, 1.3f);
        
        while (transform.position.x < TargetPoint.x)
        {
            float XPos = transform.position.x + Time.deltaTime * Toolbox.Instance.LevelSpeed * 1.7f;
            float XPercent = 1 - (TargetPoint.x - transform.position.x) / (TargetPoint.x - StartPoint.x);
            float YPos = StartPoint.y + (TargetPoint.y - StartPoint.y) * XPercent * XPercent * XPercent;
            transform.position = new Vector3(XPos, YPos, transform.position.z);
            yield return null;
        }
    }

    private IEnumerator LevelStartCountdown()
    {
        const float TimeToReachDest = 0.6f;
        const float CountdownDuration = 3f - TimeToReachDest;
        float CountdownTimer = 0f;

        yield return new WaitForSeconds(GameStartupTime);
        Level.GameMenu.RemoveLoadingOverlay();
        yield return null;

        Fog.StartOfLevel();

        bool bEntranceAnimStarted = false;
        while (CountdownTimer < CountdownDuration + TimeToReachDest)
        {
            CountdownTimer += Time.deltaTime;
            if (Level.Stats.CompletionData.ToolTipComplete(CompletionDataControl.ToolTipID.SecondJump))
            {
                Level.GameHUD.SetResumeTimer(CountdownDuration - CountdownTimer + TimeToReachDest);
            }
            else if (!bEntranceAnimStarted)
            {
                CountdownTimer = CountdownDuration;
            }

            if (CountdownTimer >= CountdownDuration && !bEntranceAnimStarted)
            {
                bEntranceAnimStarted = true;
                StartCoroutine("CaveEntranceAnimation");
            }
            yield return null;
        }

        StartGame();
        PlayerRigidBody.velocity = new Vector2(0f, JumpForce.y / 80);
        if (Level.Stats.CompletionData.ToolTipComplete(CompletionDataControl.ToolTipID.SecondJump))
        {
            Level.GameHUD.SetStartText("GO!");
            yield return new WaitForSeconds(0.6f);
        }

        Level.GameHUD.HideResumeTimer();
    }

    private IEnumerator ToolTipPause(CompletionDataControl.ToolTipID ttID)
    {
        bToolTipWait = true;
        Level.Stats.CompletionData.ShowToolTip(ttID);
        const float TooltipPauseDuration = 0.7f;

        PauseGame(ShowMenu: false);
        yield return new WaitForSeconds(TooltipPauseDuration);
        bToolTipWait = false;
        Level.Stats.CompletionData.ShowTapToResume();
        InputManager.ClearInput();
        while (!InputManager.TapRegistered() && !Input.GetKeyUp("w"))
        {
            yield return null;
        }
        Level.Stats.CompletionData.HideToolTip();
        ResumeGameplay();
        if (Clearance.IsEmpty())
        {
            Jump();
        }
        else
        {
            PlayerRigidBody.velocity = Vector2.zero;
            PlayerRigidBody.AddForce(JumpForce/3);
        }
        Level.Stats.CompletionData.SetToolTipComplete(ttID);
    }
}

