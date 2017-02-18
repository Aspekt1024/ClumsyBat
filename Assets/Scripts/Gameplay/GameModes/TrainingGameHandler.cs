using UnityEngine;
using System.Collections;

public class TrainingGameHandler : GameHandler
{
    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;
    private GameUI _gameHud;

    public int TrainingLevelNum = 1;
    private const float ResumeTimerDuration = 3f;
    private float _resumeTimerStart;
    private bool _bPaused;

    private void Start()
    {
        GameData.Instance.Level = LevelProgressionHandler.Levels.Training1;
        _loadScreen = FindObjectOfType<LoadScreen>();
        _gameHud = FindObjectOfType<GameUI>();
        _gameMenu = FindObjectOfType<GameMenuOverlay>();
        _gameMenu.Hide();
        ThePlayer.Fog.Disable();
        StartCoroutine("LoadSequence");
    }

    private void Update()
    {
        if (_bPaused) return;
    }

    private IEnumerator LoadSequence()
    {
        yield return new WaitForSeconds(1f);
        StartGame();
    }

    private void StartGame()
    {
        _gameHud.StartGame();
        _loadScreen.HideLoadScreen();
        PlayerController.EnterGamePlay();
    }

    public override void PauseGame(bool showMenu)
    {
        _bPaused = true;
        ThePlayer.PauseGame(true);
        _gameHud.GamePaused(true);
        if (showMenu)
        {
            _gameMenu.PauseGame();
        }
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
        StartCoroutine("TrainingComplete");
    }

    private IEnumerator TrainingComplete()
    {
        yield return new WaitForSeconds(2f);
        _gameMenu.WinGame();
    }

    public override void TriggerEntered(Collider2D other)
    {
        //switch (other.name)
        {
        }
    }

    public override void TriggerExited(Collider2D other)
    {
        //switch (other.name)
        {
        }
    }
}
