using UnityEngine;
using System.Collections;

public class BossGameHandler : GameHandler {

    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;
    private GameUI _gameHud;

    private EvilClumsy _theBoss;
    private BossMoths _mothControl;
    
    public int BossLevelNum = 1;
    private const float ResumeTimerDuration = 3f;
    private float _resumeTimerStart;

    private void Start()
    {
        Toolbox.Instance.Level = (int)Toolbox.LevelPrefix.BossPrefix + BossLevelNum;
        _loadScreen = FindObjectOfType<LoadScreen>();
        _gameHud = FindObjectOfType<GameUI>();
        _gameMenu = FindObjectOfType<GameMenuOverlay>();
        _theBoss = FindObjectOfType<EvilClumsy>();
        _mothControl = new GameObject("SceneSpawnables").AddComponent<BossMoths>();
        _gameMenu.Hide();
        ThePlayer.Fog.Disable();
        StartCoroutine("LoadSequence");
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
    }

    public override void PauseGame(bool showMenu)
    {
        ThePlayer.PauseGame(true);
        _theBoss.PauseGame(true);
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
        _theBoss.PauseGame(false);
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
    }

    public override void TriggerEntered(Collider2D other)
    {
        switch (other.name)
        {
            case "Projectile":
                ThePlayer.Die();
                ThePlayer.GetComponent<Rigidbody2D>().constraints -= RigidbodyConstraints2D.FreezePositionX;
                ThePlayer.GetComponent<Rigidbody2D>().velocity = new Vector2(-3f, 4f);
                break;
            case "MothTrigger":
                Moth moth = other.GetComponentInParent<Moth>();
                moth.ConsumeMoth();
                break;
        }
    }

    public override void GameOver()
    {
        _gameHud.GameOver();
        _gameMenu.GameOver();
    }
}
