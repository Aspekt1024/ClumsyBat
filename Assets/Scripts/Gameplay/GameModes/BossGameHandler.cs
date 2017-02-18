using UnityEngine;
using System.Collections;

public class BossGameHandler : GameHandler {

    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;
    private GameUI _gameHud;

    private BossHandler _theBoss;
    private BossMoths _mothControl;
    
    public LevelProgressionHandler.Levels Level = LevelProgressionHandler.Levels.Boss1;
    private const float ResumeTimerDuration = 3f;
    private float _resumeTimerStart;

    private void Start()
    {
        if (GameData.Instance.Level == LevelProgressionHandler.Levels.Unassigned)
        {
            GameData.Instance.Level = Level;
        }
        _loadScreen = FindObjectOfType<LoadScreen>();
        _gameHud = FindObjectOfType<GameUI>();
        _gameMenu = FindObjectOfType<GameMenuOverlay>();
        _mothControl = new GameObject("SceneSpawnables").AddComponent<BossMoths>();

        LoadBoss();

        _gameMenu.Hide();
        ThePlayer.Fog.Disable();
        StartCoroutine("LoadSequence");
    }
	
    private void LoadBoss()
    {
        _theBoss = new GameObject("BossNPC").AddComponent<BossHandler>();
        _theBoss.SpawnLevelBoss(Level);
    }

	private void Update ()
	{
	}
    
    private IEnumerator LoadSequence()
    {
        yield return new WaitForSeconds(1f);
        StartGame();
    }

    public override void StartGame()    // TODO this doesnt need to be implemented.
    {
        _gameHud.StartGame();
        _loadScreen.HideLoadScreen();
        PlayerController.EnterGamePlay();
        GameMusic.PlaySound(GameMusicControl.GameTrack.Boss);
        ThePlayer.SetMovementMode(FlapComponent.MovementMode.HorizontalEnabled);
    }

    public override void PauseGame(bool showMenu)
    {
        ThePlayer.PauseGame(true);
        _gameHud.GamePaused(true);
        _mothControl.PauseGame(true);
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
        ThePlayer.PauseGame(false);
        _gameHud.HideResumeTimer();
        _gameHud.GamePaused(false);
        _mothControl.PauseGame(false);
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

    public override void TriggerEntered(Collider2D other)
    {
        // TODO remove searching by name. Use tag.
        switch (other.name)
        {
            case "MothTrigger":
                Moth moth = other.GetComponentInParent<Moth>();
                moth.ConsumeMoth();
                break;
        }
        switch (other.tag)
        {
            case "Projectile":
                ThePlayer.Die();
                ThePlayer.GetComponent<Rigidbody2D>().velocity = new Vector2(-3f, 4f);
                break;
            case "Moth":
                Moth moth = other.GetComponentInParent<Moth>();
                moth.ConsumeMoth();
                break;
            default:
                Debug.Log(other.tag);
                break;
        }
    }

    public override void GameOver()
    {
        _gameHud.GameOver();
        _gameMenu.GameOver();
    }
}
