using UnityEngine;
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
        StartCoroutine("LoadSequence");
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

    private IEnumerator LoadSequence()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine("LevelStartAnimation");
        GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
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
        GameState = GameStates.Paused;
        Level.PauseGame(showMenu);
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
        PlayerController.ResumeGameplay();
        Level.GameHud.HideResumeTimer();
        Level.ResumeGame();
    }

    public override void UpdateGameSpeed(float speed)
    {
        Level.UpdateGameSpeed(speed);
    }

    protected override void OnDeath()
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
