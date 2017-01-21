using UnityEngine;
using System.Collections;

public class BossGameHandler : GameHandler {

    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;
    private GameUI _gameHud;

    private EvilClumsy _theBoss;

    private bool _bPaused;
    private const float ResumeTimerDuration = 3f;
    private float _resumeTimerStart;

    private void Start()
    {
        _loadScreen = FindObjectOfType<LoadScreen>();
        _gameHud = FindObjectOfType<GameUI>();
        _gameMenu = FindObjectOfType<GameMenuOverlay>();
        _theBoss = FindObjectOfType<EvilClumsy>();
        _gameMenu.Hide();
        ThePlayer.Fog.Disable();
        StartCoroutine("LoadSequence");
    }
	
	private void Update ()
	{
	    KeepClumsyOnScreen();
	}

    private void KeepClumsyOnScreen()
    {
        if (_bPaused || !ThePlayer.IsAlive()) { return; }

        const float minY = -2f;
        const float maxY = 4f;
        if (ThePlayer.transform.position.y < minY)
        {
            ThePlayer.transform.position -= new Vector3(0f, ThePlayer.transform.position.y - minY, 0f);
        }
        if (ThePlayer.transform.position.y > maxY)
        {
            ThePlayer.transform.position -= new Vector3(0f, ThePlayer.transform.position.y - maxY);
        }
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
    }

    public override void PauseGame(bool showMenu)
    {
        _bPaused = true;
        ThePlayer.PauseGame(true);
        _theBoss.PauseGame(true);
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
        _bPaused = false;
        ThePlayer.PauseGame(false);
        _theBoss.PauseGame(false);
        _gameHud.HideResumeTimer();
        _gameHud.GamePaused(false);
        PlayerController.ResumeGameplay();
    }

    public override void UpdateGameSpeed(float speed)
    {
    }

    public override void LevelComplete()
    {
    }

    public override void TriggerEntered(Collider2D other)
    {
        switch (other.name)
        {
            case "Projectile":
                ThePlayer.Die();
                break;
            case "MothTrigger":
                Debug.Log("moth :)");
                break;
        }
    }

    public override void GameOver()
    {
        _gameHud.GameOver();
        _gameMenu.GameOver();
    }
}
