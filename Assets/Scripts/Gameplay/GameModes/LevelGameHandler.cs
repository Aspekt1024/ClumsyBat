using UnityEngine;
using System.Collections;
using System;

public class LevelGameHandler : GameHandler
{
    [HideInInspector] public LevelScript Level;
    [HideInInspector] public CaveHandler CaveHandler;

    private float _resumeTimerStart;
    private const float ResumeTimer = 3f;
    private const float LevelStartupTime = 1f;
    private VillageSequencer _villageSequencer;
    private bool caveExitAutoFlightTriggered;
    
    private void Start ()
    {
        Level = FindObjectOfType<LevelScript>();
        ThePlayer.transform.position = new Vector3(-Toolbox.TileSizeX / 2f, 0f, ThePlayer.transform.position.z);
        CaveHandler = FindObjectOfType<CaveHandler>();
        _villageSequencer = GameObject.FindGameObjectWithTag("Scripts").AddComponent<VillageSequencer>();
        StartCoroutine("LoadSequence");
    }

    protected override void SetCameraEndPoint()
    {
        Toolbox.PlayerCam.SetEndPoint(CaveHandler.GetEndCave().transform.position.x);
    }

    private IEnumerator LoadSequence()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine("LevelStartAnimation");
        GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
        SetCameraEndPoint();
    }

    private IEnumerator LevelStartAnimation()
    {
        yield return new WaitForSeconds(LevelStartupTime);
        Level.GameMenu.RemoveLoadingOverlay();
        yield return null;

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

    public sealed override void PauseGame(bool showMenu)
    {
        EventListener.PauseGame();
        GameState = GameStates.Paused;
        Toolbox.Instance.GamePaused = true;
        Level.PauseGame(showMenu);
        ThePlayer.PauseGame();
        GameData.Instance.Data.SaveData();
    }

    public sealed override void ResumeGame(bool immediate = false)
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
        float waitTime = Level.GameMenu.RaiseMenu();
        yield return new WaitForSeconds(waitTime);
        _resumeTimerStart = Time.time;

        while (ThePlayer.IsAlive() && _resumeTimerStart + ResumeTimer - waitTime > Time.time)
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
        Level.HorribleDeath();
    }

    public override void Collision(Collision2D other)
    {
        ThePlayer.DeactivateRush();

        if (other.collider.tag == "Stalactite") { other.collider.GetComponentInParent<Stalactite>().Crack(); }

        if (ThePlayer.ActivateShield())
        {
            GameData.Instance.IsUntouched = false;
            ThePlayer.PlaySound(ClumsyAudioControl.PlayerSounds.Collision); // TODO sounds
        }
        else
        {
            if (other.collider.tag == "Stalactite")
            {
                GameData.Instance.Data.Stats.ToothDeaths++;
            }
            else
            {
                //Level.Stats.RockDeaths++; // TODO check for other objects
            }
            ThePlayer.PlaySound(ClumsyAudioControl.PlayerSounds.Collision);
            ThePlayer.Die();
        }
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
