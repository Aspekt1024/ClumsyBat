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

    private GameObject InputObject;
    private SwipeManager InputManager;

    private Vector3 PlayerStartPos = new Vector3(-5.5f, -1.5f, -2f);
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
    private bool bAtEnd = false;
    private float ResumeTimerStart;
    private float ResumeTimer = 3f;
    private Vector2 SavedVelocity = Vector2.zero;

    private bool bIsAlive = true;
    private bool bIsRushing = false;
    private bool bGameOverOverlay = false;

    public Vector2 JumpForce = new Vector2(0, 700);
    private const float GravityScale = 3;
    private float PlayerSpeed;
    private float RushStartTime;
    private const float RushTimer = 0.26f;
    private const float RushSpeed = 7f;
    
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
                Level.AddDistance(Time.deltaTime, PlayerSpeed, bIsRushing);
                if (Time.time - RushStartTime > RushTimer && bIsRushing)
                {
                    StartCoroutine("EndRushAnim");
                }
                if (Level.AtCaveEnd())
                {
                    bAtEnd = true;
                    transform.position += new Vector3(Time.deltaTime * PlayerSpeed * Toolbox.Instance.LevelSpeed, 0f, 0f);
                }
            }

            if (InputManager.SwipeRegistered())
            {
                Rush();
            }
            if (InputManager.TapRegistered())
            {
                Jump();
            }
        }

        // For debugging
        if (Input.GetKeyUp("w"))
        {
            Jump();
        } else if (Input.GetKeyUp("a"))
        {
            Rush();
        }

        // Die by being off-screen
        if (!bGameStarted) { return; }
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
                Level.ShowGameoverMenu();
                bGameOverOverlay = true;
            }
        }

        if (transform.position.y < 1f && !Level.Stats.CompletionData.ToolTipComplete(CompletionDataControl.ToolTipID.SecondJump))
        {
            StartCoroutine("ToolTipPause", CompletionDataControl.ToolTipID.SecondJump);
        }
    }

    void Jump()
    {
        if (!Level.Stats.CompletionData.ToolTipComplete(CompletionDataControl.ToolTipID.SecondJump) || bToolTipWait)
        {
            return;
        }

        if (!bIsAlive || !bGameStarted) { return; }
        
        if (bGamePaused) { ResumeGame(); }

        Level.Stats.TotalJumps++;
        PlayerRigidBody.velocity = Vector2.zero;
        PlayerRigidBody.AddForce(JumpForce);
        //GetComponent<AudioSource>().Play();
        Anim.Play("Flap", 0, 0.5f);

    }

    void Rush()
    {
        if (!bIsAlive || bIsRushing || bGamePaused || !bGameStarted || bToolTipWait) { return; }
        Level.Stats.TimesDashed++;
        StartCoroutine("StartRushAnim");
        // TODO Rush cooldown and failed rush when on cooldown?
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

        switch (MothScript.Colour)
        {
            case Moth.MothColour.Green:
                Lantern.ChangeColour(Lantern.LanternColour.Green);
                Fog.Echolocate();
                break;
            case Moth.MothColour.Gold:
                Lantern.ChangeColour(Lantern.LanternColour.Gold);
                StartHypersonic();
                Fog.Echolocate();
                break;
            case Moth.MothColour.Blue:
                Lantern.ChangeColour(Lantern.LanternColour.Blue);
                Fog.Echolocate();
                break;
        }
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
        Lantern.PauseHinge();
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
        ResumeTimerStart = Time.time;
        bGameResuming = true;
        StartCoroutine("UpdateResumeTimer");
    }

    IEnumerator UpdateResumeTimer()
    {
        Level.GameMenu.Hide();
        while (ResumeTimerStart + ResumeTimer > Time.time)
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
        Lantern.ResumeHinge();
    }

    IEnumerator StartRushAnim()
    {
        bIsRushing = true;
        RushStartTime = Time.time;
        PlayerRigidBody.gravityScale = 0f;
        PlayerRigidBody.velocity = new Vector2(8f, 0f);

        Anim.Play("Rush", 0, 0);
        Lantern.AddRushForce();

        yield return new WaitForSeconds(0.07f);

        if (bIsAlive)
        {
            PlayerSpeed = RushSpeed;
            Level.UpdateGameSpeed(PlayerSpeed);
            PlayerRigidBody.velocity = Vector2.zero;
        }
    }

    IEnumerator EndRushAnim()
    {
        PlayerSpeed = 1f;
        Level.UpdateGameSpeed(PlayerSpeed);
        bIsRushing = false;
        PlayerRigidBody.gravityScale = GravityScale;
        
        while (PlayerRigidBody.position.x > PlayerStartPos.x && !bAtEnd)
        {
            float NewSpeed = 3 - (2*PlayerRigidBody.position.x / (PlayerRigidBody.position.x + PlayerStartPos.x));
            PlayerRigidBody.velocity = new Vector2(-NewSpeed, PlayerRigidBody.velocity.y);
            yield return null;
        }
        PlayerRigidBody.velocity = new Vector2(0, PlayerRigidBody.velocity.y);
        Anim.Play("Flap", 0, 0);
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
        Vector3 StartPoint = new Vector3(-Toolbox.TileSizeX / 2f, 0f, transform.position.z);
        Vector2 TargetPoint = new Vector2(-5f, 1.3f);
        transform.position = StartPoint;
        
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
        while (!InputManager.TapRegistered() && !Input.GetKeyUp("space"))
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
    }
}

