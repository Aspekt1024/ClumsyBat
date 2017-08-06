using UnityEngine;

using GameState = GameHandler.GameStates;

/// <summary>
/// Handles the player input (touch screen)
/// Determines how the player jumps and what happens when they hit an object
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Player ThePlayer;
    public GameState State;

    public enum SwipeDirecitons
    {
        Left, Right
    }

    private GameHandler _gameHandler;
    private InputManager _inputManager;

    private bool _bTouchInputEnabled = true;
    private bool _bTouchHeld;

    private bool isAwaitingInput;

    private void Awake()
    {
        State = GameState.Starting;
        var scriptsObject = GameObject.Find("Scripts");
        _inputManager = scriptsObject.AddComponent<InputManager>();
        _gameHandler = scriptsObject.GetComponent<GameHandler>();
        ThePlayer = FindObjectOfType<Player>();
    }
    
    private void Update()
    {
        if (State != GameState.Normal || !ThePlayer.IsAlive()) { return; }

        if (!_bTouchHeld && (ThePlayer.IsPerchedOnTop() || ThePlayer.TouchReleasedOnBottom()))
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
        if (!_bTouchInputEnabled || State == GameState.Paused) { return; }
        
        if (_inputManager.SwipeLeftRegistered() && GameData.Instance.IsBossLevel())
            ProcessSwipe(SwipeDirecitons.Left);

        if (_inputManager.SwipeRightRegistered())
            ProcessSwipe(SwipeDirecitons.Right);

        if (_inputManager.TapRegistered())
            ProcessTap();
    }

    private void ProcessTap()
    {
        if (isAwaitingInput)
            isAwaitingInput = false;
        
        InputManager.TapDirection tapDir = _inputManager.GetTapDir();
        ThePlayer.ActivateJump(tapDir);
    }

    private void ProcessSwipe(SwipeDirecitons direction)
    {
        if (ThePlayer.CanRush())
        {
            ThePlayer.ActivateRush(direction);
        }
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

    public void WaitForVillageSpeech()
    {
        State = GameState.PausedForTooltip;
        _inputManager.ClearInput();
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
        _inputManager.ClearInput();
        _bTouchInputEnabled = !bPaused;
    }

    public void WaitForInput()
    {
        isAwaitingInput = true;
        _inputManager.ClearInput();
    }

    public bool WaitingForInput()
    {
        return isAwaitingInput;
    }

    public bool GameStarted() { return State != GameState.Starting; }
    public bool TouchHeld() { return _inputManager.TouchHeld(); }
    public InputManager GetInputManager() { return _inputManager; }
    public bool InputPaused() { return !_bTouchInputEnabled; }
}

