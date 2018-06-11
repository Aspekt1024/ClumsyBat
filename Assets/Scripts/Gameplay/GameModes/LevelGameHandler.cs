using UnityEngine;
using System.Collections;
using System;

public sealed class LevelGameHandler : GameHandler
{
    [HideInInspector] public LevelScript Level;
    [HideInInspector] public CaveHandler CaveHandler;

    private float _resumeTimerStart;
    private const float ResumeTimer = 3f;
    private bool caveExitAutoFlightTriggered;

    private void Start()
    {
        Level = FindObjectOfType<LevelScript>();
        CaveHandler = FindObjectOfType<CaveHandler>();
    }

    public void StartLevel()
    {
        ThePlayer.transform.position = new Vector3(-Toolbox.TileSizeX / 2f, 0f, ThePlayer.transform.position.z);

        StartCoroutine(LevelStartAnimation());
        GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
        SetCameraEndPoint();
        Level.GameHud.SetCurrencyText("0/" + GameData.Instance.NumMoths);
    }

    protected override void SetCameraEndPoint()
    {
        Toolbox.PlayerCam.SetEndPoint(CaveHandler.GetEndCave().transform.position.x);
    }
    
    private IEnumerator LevelStartAnimation()
    {

        ThePlayer.StartFog();
        ThePlayer.StartCoroutine("CaveEntranceAnimation");

        const float timeToReachDest = 0.6f;
        yield return new WaitForSeconds(timeToReachDest);

        LevelStart();
    }

    private void LevelStart()
    {
        PlayerController.EnterGamePlay();
        Level.StartGame();
    }

    public override void PauseGame()
    {
        EventListener.PauseGame();
        GameState = GameStates.Paused;
        Toolbox.Instance.GamePaused = true;
        Level.PauseGame();
        ThePlayer.PauseGame();
        GameData.Instance.Data.SaveData();
    }

    public override void ResumeGame(bool immediate = false)
    {
        if (immediate)
        {
            ResumeGameplay();
        }
        else
        {
            GameData.Instance.Data.SaveData();
            StartCoroutine("UpdateResumeTimer");
        }
    }

    private IEnumerator UpdateResumeTimer()
    {
        // TODO raise menu
        _resumeTimerStart = Time.time;

        while (ThePlayer.IsAlive() && _resumeTimerStart + ResumeTimer > Time.time)
        {
            float timeRemaining = _resumeTimerStart + ResumeTimer - Time.time;
            Level.GameHud.SetResumeTimer(timeRemaining);
            yield return null;
        }
        ResumeGameplay();
    }

    public void ResumeGameplay()
    {
        GameState = GameStates.Normal;
        Toolbox.Instance.GamePaused = false;
        EventListener.ResumeGame();
        PlayerController.ResumeGameplay();
        ThePlayer.ResumeGame();
        Level.GameHud.HideResumeTimer();
        Level.ResumeGame();
    }

    protected override void OnDeath()
    {
        GetComponent<AudioSource>().Stop();
    }

    public override void TriggerEntered(Collider2D other)
    {
        switch (other.name)
        {
            case "MothTrigger":
                other.GetComponentInParent<Moth>().ConsumeMoth();
                break;
            case "ExitTrigger":
                if (!caveExitAutoFlightTriggered)
                {
                    caveExitAutoFlightTriggered = true;
                    ThePlayer.ExitAutoFlightReached();
                }
                break;
            case "Spore":
                ThePlayer.Fog.Minimise();
                break;
        }
    }

    public override void LevelComplete()
    {
        Level.LevelWon();
    }

    public override void GameOver()
    {
        Level.ShowGameoverMenu();
    }

    public override MothPool GetMothPool()
    {
        // TODO implement this?
        return null;
    }
}
