using UnityEngine;
using System;
using System.Collections;

public delegate void PlayerDeathHandler(object sender, EventArgs e);

/// <summary>
/// Handles the player input (touch screen)
/// Determines how the player jumps and what happens when they hit an object
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Player ThePlayer;
    public LevelScript Level;

    private GameObject InputObject;
    private SwipeManager InputManager;

    private const float GameStartupTime = 1f;
    
    private float ResumeTimerStart;
    private float ResumeTimer = 3f;

    private enum GameState
    {
        Starting,
        Normal,
        Paused,
        Resuming,
        PausedForTooltip
    }
    private GameState State = GameState.Starting;
    private bool bTouchInputEnabled = true;

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
        InputObject = new GameObject("Game Scripts");
        InputManager = InputObject.AddComponent<SwipeManager>();
        ThePlayer = FindObjectOfType<Player>();
    }

    public void LevelStart()
    {
        StartCoroutine("LevelStartCountdown");
    }
    
    void Update()
    {
        if (State == GameState.Paused || State == GameState.Resuming) { return; }

        if (ThePlayer.IsAlive())
        {
            if (State == GameState.Normal)
            {
                if (Level.AtCaveEnd())
                {
                    ThePlayer.CaveEndReached();
                }
            }
            ProcessInput();
        }
    }

    private void ProcessInput()
    {
        if (State == GameState.PausedForTooltip || State == GameState.Starting || !bTouchInputEnabled) { return; }

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
        if (State == GameState.Paused) { ResumeGame(); }
        ThePlayer.ActivateJump();
    }

    private void ProcessSwipe()
    {
        if (State == GameState.Paused) { return; }
        ThePlayer.ActivateRush();
    }

    void StartGame()
    {
        State = GameState.Normal;
        ThePlayer.StartGame();
        Level.StartGame();
    }

    public void PauseButtonPressed()
    {
        PauseGame(ShowMenu: true);
    }

    public void PauseGame(bool ShowMenu = true)
    {
        State = GameState.Paused;
        Level.PauseGame(ShowMenu);
        ThePlayer.PauseGame(true);
        Level.Stats.SaveStats();
    }

    public void ResumeGame()
    {
        if (State == GameState.Resuming) { return; }
        Level.Stats.SaveStats();
        State = GameState.Resuming;
        
        StartCoroutine("UpdateResumeTimer");
    }

    IEnumerator UpdateResumeTimer()
    {
        float WaitTime = Level.GameMenu.RaiseMenu();
        yield return new WaitForSeconds(WaitTime);
        ResumeTimerStart = Time.time;
        
        while (ThePlayer.IsAlive() && ResumeTimerStart + ResumeTimer - WaitTime > Time.time)
        {
            float TimeRemaining = ResumeTimerStart + ResumeTimer - Time.time;
            Level.GameHUD.SetResumeTimer(TimeRemaining);
            yield return null;
        }
        ResumeGameplay();
    }

    void ResumeGameplay()
    {
        State = GameState.Normal;
        InputManager.ClearInput();
        Level.GameHUD.HideResumeTimer();
        Level.ResumeGame();
        ThePlayer.PauseGame(false);
    }

    private IEnumerator LevelStartCountdown()
    {
        const float TimeToReachDest = 0.6f;
        const float CountdownDuration = 3f - TimeToReachDest;
        float CountdownTimer = 0f;

        yield return new WaitForSeconds(GameStartupTime);
        Level.GameMenu.RemoveLoadingOverlay();
        yield return null;

        ThePlayer.StartFog();

        bool bEntranceAnimStarted = false;
        while (CountdownTimer < CountdownDuration + TimeToReachDest)
        {
            CountdownTimer += Time.deltaTime;

            // First level is special (tutorial) so we're going to change the animation for this one only
            if (!VeryFirstStartupSequenceRequired())
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
                ThePlayer.StartCoroutine("CaveEntranceAnimation");
            }
            yield return null;
        }

        StartGame();
        if (!VeryFirstStartupSequenceRequired())
        {
            Level.GameHUD.SetStartText("GO!");
            yield return new WaitForSeconds(0.6f);
        }

        Level.GameHUD.HideResumeTimer();
    }

    public bool VeryFirstStartupSequenceRequired()
    {
        if (Toolbox.Instance.Level == 1
            && !Toolbox.Instance.TooltipCompleted(TooltipHandler.DialogueID.FirstJump)
            && Toolbox.Instance.ShowLevelTooltips)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void WaitForTooltip(bool bWait)
    {
        State = GameState.PausedForTooltip;
        InputManager.ClearInput();

        if (bWait)
        {
            PauseGame(ShowMenu: false);
        }
    }

    public void TooltipResume()
    {
        ResumeGameplay();
        ThePlayer.JumpIfClear();
    }

    public SwipeManager GetInputManager()
    {
        return InputManager;
    }

    public bool GameStarted()
    {
        return (State == GameState.Starting ? false : true);
    }

    public void PauseInput(bool bPaused)
    {
        // This is currently only used by the Gameover sequence and is reset upon loading the scene
        bTouchInputEnabled = !bPaused;
    }
}

