using UnityEngine;
using System.Collections;

public class BossGameHandler : GameHandler {

    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;
    private GameUI _gameHud;
    private Camera _playerCam;
    
    public LevelProgressionHandler.Levels Level = LevelProgressionHandler.Levels.Boss1;
    private const float ResumeTimerDuration = 3f;
    private float _resumeTimerStart;

    private float _distTravelled;

    private void OnEnable()
    {
        EventListener.OnLevelWon += LevelComplete;
    }
    private void OnDisable()
    {
        EventListener.OnLevelWon -= LevelComplete;
    }

    private enum BossGameState
    {
        Startup,
        MovingTowardsBoss,
        InBossRoom
    }
    private BossGameState _state;

    private void Start()
    {
        if (GameData.Instance.Level == LevelProgressionHandler.Levels.Unassigned)
        {
            GameData.Instance.Level = Level;
        }
        _loadScreen = FindObjectOfType<LoadScreen>();
        _gameHud = FindObjectOfType<GameUI>();
        _gameMenu = FindObjectOfType<GameMenuOverlay>();
        _playerCam = FindObjectOfType<Camera>();

        _gameMenu.Hide();
        ThePlayer.Fog.Disable();
        StartCoroutine("LoadSequence");
    }

	private void Update ()
	{
        if (GameState != GameStates.Normal) return;
        if (_state == BossGameState.MovingTowardsBoss)
        {
            MoveClumsy(Time.deltaTime);
        }
	}

    private void MoveClumsy(float time)
    {
        const float manualCaveScale = 0.8558578f;
        float distToTravel = Toolbox.TileSizeX * manualCaveScale + 1f;
        if (ThePlayer.IsPerched()) return;
        float dist = time * ThePlayer.GetPlayerSpeed();
        if (_distTravelled + dist > distToTravel)
        {
            dist = distToTravel - _distTravelled;
            ThePlayer.SetMovementMode(FlapComponent.MovementMode.HorizontalEnabled);
            StartCoroutine("BossEntrance");
        }
        _distTravelled += dist;
        ThePlayer.transform.position += Vector3.right * dist;
        _playerCam.transform.position += Vector3.right * dist;
        ThePlayer.Lantern.transform.position += Vector3.right * dist;
    }
    
    private IEnumerator LoadSequence()
    {
        yield return new WaitForSeconds(1f);
        StartGame();
        yield return ThePlayer.StartCoroutine("CaveEntranceAnimation");

        // TODO put this into a function that says "boss level begin" or something
        GameState = GameStates.Normal;
        _state = BossGameState.MovingTowardsBoss;
        ThePlayer.SetPlayerSpeed(Toolbox.Instance.LevelSpeed);
        PlayerController.EnterGamePlay();
    }

    private IEnumerator BossEntrance()
    {
        _state = BossGameState.InBossRoom;
        ThePlayer.EnableHover();

        FindObjectOfType<SlidingDoors>().Close();
        
        // TODO boss entrance sequence
        yield return new WaitForSeconds(2f);
        ThePlayer.DisableHover();
        BossEvents.BossFightStart();
    }

    private void StartGame()
    {
        _gameHud.StartGame();
        _loadScreen.HideLoadScreen();
        GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
        ThePlayer.SetMovementMode(FlapComponent.MovementMode.VerticalOnly);
    }

    public override void PauseGame(bool showMenu)
    {
        EventListener.PauseGame();
        Toolbox.Instance.GamePaused = true;
        GameState = GameStates.Paused;
        ThePlayer.PauseGame(true);
        _gameHud.GamePaused(true);
        if (showMenu) { _gameMenu.PauseGame(); }
    }

    public override void ResumeGame(bool immediate = false)
    {
        if (!immediate)
        {
            _gameMenu.RaiseMenu();
            StartCoroutine("UpdateResumeTimer");
        }
        else
        {
            _gameMenu.Hide();
            ResumeGameplay();
        }
    }
    
    private IEnumerator UpdateResumeTimer()
    {
        GameState = GameStates.Resuming;
        float waitTime = _gameMenu.RaiseMenu();
        yield return new WaitForSeconds(waitTime);
        _resumeTimerStart = Time.time;

        while (ThePlayer.IsAlive() && _resumeTimerStart + ResumeTimerDuration - waitTime > Time.time)
        {
            float timeRemaining = _resumeTimerStart + ResumeTimerDuration - Time.time;
            _gameHud.SetResumeTimer(timeRemaining);
            yield return null;
        }
        ResumeGameplay();
    }

    private void ResumeGameplay()
    {
        EventListener.ResumeGame();
        Toolbox.Instance.GamePaused = false;
        GameState = GameStates.Normal;
        ThePlayer.PauseGame(false);
        _gameHud.HideResumeTimer();
        _gameHud.GamePaused(false);
        PlayerController.ResumeGameplay();
    }

    public override void UpdateGameSpeed(float speed)
    {
    }

    public override void LevelComplete()
    {
        _gameHud.LevelWon();
        StartCoroutine("BossFightWon");
    }

    private IEnumerator BossFightWon()
    {
        yield return new WaitForSeconds(2f);
        _gameMenu.WinGame();
        GameData.Instance.SetLevelCompletion(GameData.LevelCompletePaths.MainPath);
        EventListener.LevelWon();

        // TODO add sound to sound controller script
    }

    public override void GameOver()
    {
        _gameHud.GameOver();
        _gameMenu.GameOver();
    }
}
