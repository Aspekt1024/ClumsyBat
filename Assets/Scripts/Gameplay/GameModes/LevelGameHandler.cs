using UnityEngine;
using System.Collections;

using StoryEventID = StoryEventControl.StoryEvents;

public class LevelGameHandler : GameHandler
{
    public LevelScript Level;
    
    private float _resumeTimerStart;
    private const float ResumeTimer = 3f;
    private const float LevelStartupTime = 1f;

    private void Start ()
    {
        Level = FindObjectOfType<LevelScript>();
        ThePlayer.transform.position = new Vector3(-Toolbox.TileSizeX / 2f, 0f, ThePlayer.transform.position.z);
    }
	
	private void Update ()
    {
        if (ThePlayer.IsAlive())
        {
            Level.AddDistance(Time.deltaTime, ThePlayer.GetPlayerSpeed());
        }
        if (PlayerController.State == GameStates.Normal && Level.AtCaveEnd())
        {
            ThePlayer.CaveEndReached();
        }
    }

    public sealed override void StartGame()
    {
        StartCoroutine("LevelStartCountdown");
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
                Level.Stats.RockDeaths++; // TODO check for other objects
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
                StartCoroutine("ConsumeMoth", other);
                break;
            case "ExitTrigger":
                ThePlayer.ExitAutoFlightReached();
                break;
            case "Spore":
                ThePlayer.Fog.Minimise();
                break;
        }
    }

    // TODO move to consume moth handler or something
    private IEnumerator ConsumeMoth(Collider2D mothCollider)
    {
        if (Level.Stats.MothsEaten > Level.Stats.MostMoths)
        {
            Level.Stats.MostMoths++;
        }
        Level.Stats.TotalMoths++;
        Moth mothScript = mothCollider.GetComponentInParent<Moth>();
        float animationWaitTime = mothScript.ConsumeMoth();
        float animTimer = 0f;
        while (animTimer < animationWaitTime)
        {
            if (GameState != GameStates.Paused)
            {
                animTimer += Time.deltaTime;
            }
            yield return null;
        }

        // TODO redo this. maybe another script file...
        int currencyValue = 0;
        switch (mothScript.Colour)
        {
            case Moth.MothColour.Green:
                currencyValue = 1;
                ThePlayer.Lantern.ChangeColour(Lantern.LanternColour.Green);
                ThePlayer.Fog.Echolocate();
                break;
            case Moth.MothColour.Gold:
                currencyValue = 2;
                ThePlayer.Lantern.ChangeColour(Lantern.LanternColour.Gold);
                Level.Stats.StoryData.TriggerEvent(StoryEventID.FirstGoldMoth);
                ThePlayer.ActivateHypersonic();
                ThePlayer.Fog.Echolocate();
                break;
            case Moth.MothColour.Blue:
                currencyValue = 3;
                ThePlayer.Lantern.ChangeColour(Lantern.LanternColour.Blue);
                ThePlayer.Fog.Echolocate();
                break;
        }
        ThePlayer.AddShieldCharge();
        Level.Stats.MothsEaten++;
        Level.Stats.CollectedCurrency += currencyValue;
        Level.GameHud.UpdateCurrency(Pulse: true); // TODO move
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
