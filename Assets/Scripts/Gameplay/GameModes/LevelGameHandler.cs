﻿using UnityEngine;
using System.Collections;
public class LevelGameHandler : GameHandler
{
    public LevelScript Level;
    
    private float _resumeTimerStart;
    private const float ResumeTimer = 3f;
    private const float LevelStartupTime = 1f;
    private CaveHandler _caveHandler;
    private bool _caveGnomeEndSequenceStarted;
    private VillageSequencer _villageSequencer;

    private void Start ()
    {
        Level = FindObjectOfType<LevelScript>();
        ThePlayer.transform.position = new Vector3(-Toolbox.TileSizeX / 2f, 0f, ThePlayer.transform.position.z);
        _caveHandler = FindObjectOfType<CaveHandler>();
        _villageSequencer = GameObject.FindGameObjectWithTag("Scripts").AddComponent<VillageSequencer>();
    }
	
	private void Update ()
    {
        if (ThePlayer.IsAlive() && !Level.AtCaveEnd())
        {
            AddDistanceFromTime(Time.deltaTime);
        }
        if (PlayerController.State == GameStates.Normal && Level.AtCaveEnd())
        {
            ThePlayer.CaveEndReached();
        }
    }

    public override void MovePlayerAtCaveEnd(float dist)
    {
        if (_caveGnomeEndSequenceStarted) return;
        if (_caveHandler.IsGnomeEnding())
        {
            _caveGnomeEndSequenceStarted = true;
            GameMusic.PlaySound(GameMusicControl.GameTrack.Village);
            _villageSequencer.StartCoroutine("StartSequence");
        }
        else
        {
            base.MovePlayerAtCaveEnd(dist);
        }
    }

    private void AddDistanceFromTime(double time)
    {
        float addDist = (float)time * Level.GetGameSpeed() * Toolbox.Instance.LevelSpeed;
        AddDistance(addDist);
    }

    protected override void AddDistance(float dist)
    {
        if (_caveHandler.IsGnomeEnding() && Level.AtCaveEnd() || !ThePlayer.GameHasStarted()) return;
        base.AddDistance(dist);
    }

    public sealed override void StartGame()
    {
        StartCoroutine("LevelStartCountdown");
        GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
    }

    private IEnumerator LevelStartCountdown()
    {
        const float timeToReachDest = 0.6f;
        const float countdownDuration = 3f - timeToReachDest;
        float countdownTimer = 0f;
        
        yield return new WaitForSeconds(LevelStartupTime);
        Level.GameMenu.RemoveLoadingOverlay();
        yield return null;

        ThePlayer.StartFog();

        bool bEntranceAnimStarted = false;
        while (countdownTimer < countdownDuration + timeToReachDest)
        {
            countdownTimer += Time.deltaTime;

            // First level is special (tutorial) so we're going to change the animation for this one only
            if (!VeryFirstStartupSequenceRequired())
            {
                Level.GameHud.SetResumeTimer(countdownDuration - countdownTimer + timeToReachDest);
            }
            else if (!bEntranceAnimStarted)
            {
                countdownTimer = countdownDuration;
            }

            if (countdownTimer >= countdownDuration && !bEntranceAnimStarted)
            {
                bEntranceAnimStarted = true;
                ThePlayer.StartCoroutine("CaveEntranceAnimation");
            }
            yield return null;
        }

        LevelStart();
        if (!VeryFirstStartupSequenceRequired())
        {
            Level.GameHud.SetStartText("GO!");
            yield return new WaitForSeconds(0.6f);
        }

        Level.GameHud.HideResumeTimer();
    }

    public bool VeryFirstStartupSequenceRequired()
    {
        if (Toolbox.Instance.Level == 1
            && !Toolbox.Instance.TooltipCompleted(TooltipHandler.DialogueId.FirstJump)
            && Toolbox.Instance.ShowLevelTooltips)
        {
            return true;
        }
        return false;
    }

    private void LevelStart()
    {
        PlayerController.EnterGamePlay();
        Level.StartGame();
    }

    public sealed override void PauseGame(bool showMenu)
    {
        GameState = GameStates.Paused;
        Level.PauseGame(showMenu);
        Level.Stats.SaveStats();
    }

    public sealed override void ResumeGame(bool immediate = false)
    {
        if (immediate)
        {
            ResumeGameplay();
        }
        else
        {
            Level.Stats.SaveStats();
            StartCoroutine("UpdateResumeTimer");
        }
    }

    IEnumerator UpdateResumeTimer()
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
        PlayerController.ResumeGameplay();
        Level.GameHud.HideResumeTimer();
        Level.ResumeGame();
    }

    public override void UpdateGameSpeed(float speed)
    {
        Level.UpdateGameSpeed(speed);
    }

    public override void Death()
    {
        Level.HorribleDeath();
    }

    public override void Collision(Collision2D other)
    {
        ThePlayer.DeactivateRush();

        if (other.collider.name == "StalObject") { other.collider.GetComponentInParent<Stalactite>().Crack(); }

        if (ThePlayer.ActivateShield())
        { 
            ThePlayer.PlaySound(ClumsyAudioControl.PlayerSounds.Collision); // TODO sounds
        }
        else
        {
            if (other.collider.name == "StalObject")
            {
                Level.Stats.ToothDeaths++;
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
                ThePlayer.ExitAutoFlightReached();
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
}