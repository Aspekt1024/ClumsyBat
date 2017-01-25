using UnityEngine;
using System;

using GameState = GameHandler.GameStates;

public delegate void PlayerDeathHandler(object sender, EventArgs e);

/// <summary>
/// Handles the player input (touch screen)
/// Determines how the player jumps and what happens when they hit an object
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Player ThePlayer;
    
    private GameHandler _gameHandler;
    private InputManager _inputManager;
    
    public GameState State { get; set; }

    private bool _bTouchInputEnabled = true;
    private bool _bTouchHeld;

    public event PlayerDeathHandler PlayerDeath; // not currently used. Kept for reference (events!)
    
    protected virtual void OnDeath(EventArgs e)
    {
        if (PlayerDeath != null)
        {
            PlayerDeath(this, e);
        }
    }
    
    private void Awake()
    {
        State = GameState.Starting;
        var scriptsObject = GameObject.Find("Scripts");
        _inputManager = scriptsObject.AddComponent<InputManager>();
        _gameHandler = scriptsObject.GetComponent<GameHandler>();
        ThePlayer = FindObjectOfType<Player>();
    }

    public void StartGame()
    {
        _gameHandler.StartGame();
    }
    
    private void Update()
    {
        if (State != GameState.Normal || !ThePlayer.IsAlive()) { return; }

        if (!_bTouchHeld && ThePlayer.IsPerched())
        {
            ThePlayer.ActivateJump();
            _inputManager.ClearInput();
            return;
        }
        _bTouchHeld = _inputManager.TouchHeld();

        if (ThePlayer.IsPerched() && _bTouchHeld) { return; }
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (!_bTouchInputEnabled) { return; }
        
        if (_inputManager.SwipeRightRegistered()) { ProcessSwipe(); }
        if (_inputManager.TapRegistered()) { ProcessTap(); }
    }

    private void ProcessTap()
    {
        if (State == GameState.Paused) { return; }
        ThePlayer.ActivateJump();
    }

    private void ProcessSwipe()
    {
        if (State == GameState.Paused) { return; }
        if (ThePlayer.CanRush())
            ThePlayer.ActivateRush();
        else
            ThePlayer.ActivateJump();
    }

    public void EnterGamePlay()
    {
        State = GameState.Normal;
        ThePlayer.StartGame();
    }

    public void PauseButtonPressed()
    {
        PauseGame(showMenu: true);
    }

    public void PauseGame(bool showMenu)
    {
        State = GameState.Paused;
        ThePlayer.PauseGame(true);
        _gameHandler.PauseGame(showMenu);
    }

    public void ResumeGame()
    {
        if (State == GameState.Resuming) { return; }
        State = GameState.Resuming;
        _gameHandler.ResumeGame();
    }

    public void ResumeGameplay()
    {
        State = GameState.Normal;
        _inputManager.ClearInput();
        ThePlayer.PauseGame(false);
    }

    public void WaitForTooltip(bool bWait)
    {
        State = GameState.PausedForTooltip;
        _inputManager.ClearInput();

        if (bWait)
        {
            PauseGame(showMenu: false);
        }
    }

    public void TooltipResume()
    {
        _gameHandler.ResumeGame(immediate: true);
        ResumeGameplay();
        ThePlayer.JumpIfClear();
    }
    
    public void PauseInput(bool bPaused)
    {
        // This is currently only used by the Gameover sequence and is reset upon loading the scene
        _bTouchInputEnabled = !bPaused;
    }
    
    public bool VeryFirstStartupSequenceRequired()
    {
        var gameHandler = _gameHandler.GetComponent<LevelGameHandler>();
        if (!gameHandler) { return false; }
        return gameHandler.VeryFirstStartupSequenceRequired();
    }

    public bool GameStarted() { return State != GameState.Starting; }
    public bool TouchHeld() { return _inputManager.TouchHeld(); }
    public InputManager GetInputManager() { return _inputManager; }
}

