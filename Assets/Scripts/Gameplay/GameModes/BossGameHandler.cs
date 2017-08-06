using UnityEngine;
using System.Collections;
using System;

public class BossGameHandler : GameHandler {

    public float ClumsySpeed = 6f;

    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;
    private GameUI _gameHud;
    private Canvas leftRightOverlay;
    
    public LevelProgressionHandler.Levels Level = LevelProgressionHandler.Levels.Boss1;
    private const float ResumeTimerDuration = 3f;
    private float _resumeTimerStart;

    private bool startingDialoagueComplete;

    private const float manualCaveScale = 0.8558578f;
    
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
        
        foreach (Transform tf in Toolbox.PlayerCam.transform)
        {
            if (tf.name == "LeftRightOverlay")
            {
                leftRightOverlay = tf.GetComponent<Canvas>();
                leftRightOverlay.enabled = false;
                break;
            }
        }

        LoadBoss();

        _gameMenu.Hide();
        ThePlayer.Fog.Disable();
        SetCameraEndPoint();
        StartCoroutine(LoadSequence());
    }

	private void Update ()
    {
        if (!startingDialoagueComplete)
        {
            if (Toolbox.Player.transform.position.x > 0f)
            {
                startingDialoagueComplete = true;
                BossEntranceDialogue();
            }
        }

        if (GameState != GameStates.Normal || _state == BossGameState.InBossRoom) return;
        if (Toolbox.Player.transform.position.x > Toolbox.TileSizeX * manualCaveScale - 3f)
        {
            _state = BossGameState.InBossRoom;
            StartCoroutine(BossEntrance());
        }
	}

    protected override void SetCameraEndPoint()
    {
        Toolbox.PlayerCam.SetEndPoint(Toolbox.TileSizeX * manualCaveScale + 0.8f);
        Toolbox.PlayerCam.StopFollowingAtEndPoint();
    }
    
    private IEnumerator LoadSequence()
    {
        yield return new WaitForSeconds(1f);
        StartGame();
        yield return ThePlayer.StartCoroutine(ThePlayer.CaveEntranceAnimation());

        // TODO put this into a function that says "boss level begin" or something
        GameState = GameStates.Normal;
        _state = BossGameState.MovingTowardsBoss;
        ThePlayer.SetPlayerSpeed(ClumsySpeed);
        PlayerController.EnterGamePlay();
    }

    private IEnumerator BossEntrance()
    {
        ThePlayer.EnableHover();

        FindObjectOfType<SlidingDoors>().Close();

        // TODO boss entrance sequence
        float timer = 0f;
        const float duration = 2f;
        while (timer < duration)
        {
            if (!Toolbox.Instance.GamePaused)
                timer += Time.deltaTime;
            yield return null;
        }

        if (GameData.Instance.Level == LevelProgressionHandler.Levels.Boss1)
        {
            yield return StartCoroutine(ShowMovementTutorial());
        }

        ThePlayer.DisableHover();
        ThePlayer.SetPlayerSpeed(0);
        ThePlayer.SetMovementMode(FlapComponent.MovementMode.HorizontalEnabled);
        BossEvents.BossFightStart();
    }

    private IEnumerator ShowMovementTutorial()
    {
        if (GameData.Instance.Data.LevelData.LevelCompletedAchievement((int)LevelProgressionHandler.Levels.Boss1)
            || GameData.Instance.BossLeftRightTapTutorialSeen)
            yield break;

        GameData.Instance.BossLeftRightTapTutorialSeen = true;
        leftRightOverlay.enabled = true;
        _gameHud.GetComponent<Canvas>().enabled = false;

        yield return new WaitForSeconds(1f);

        Toolbox.Player.GetPlayerController().PauseInput(false);
        Toolbox.Player.GetPlayerController().WaitForInput();
        while (Toolbox.Player.GetPlayerController().WaitingForInput())
        {
            yield return null;
        }
        leftRightOverlay.enabled = false;
        _gameHud.GetComponent<Canvas>().enabled = true;
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
        ThePlayer.PauseGame();
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
        ThePlayer.ResumeGame();
        _gameHud.HideResumeTimer();
        _gameHud.GamePaused(false);
        PlayerController.ResumeGameplay();
    }
    
    public override void LevelComplete()
    {
        ThePlayer.EnableHover();
        ThePlayer.GetCollider().enabled = false;
        StartCoroutine(BossFightWon());
    }

    private IEnumerator BossFightWon()
    {
        if (GameData.Instance.Level == LevelProgressionHandler.Levels.Boss2)
        {
            yield return new WaitForSeconds(2.5f);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }
        
        GameData.Instance.SetLevelCompletion(GameData.LevelCompletePaths.MainPath);
        EventListener.LevelWon();
        _gameMenu.WinGame();
        _gameHud.LevelWon();

        // TODO add sound to sound controller script
    }

    public override void GameOver()
    {
        _gameHud.GameOver();
        _gameMenu.GameOver();
    }

    public override MothPool GetMothPool()
    {
        BossMoths bossMoths = GetComponent<BossMoths>();
        if (bossMoths == null)
        {
            bossMoths = gameObject.AddComponent<BossMoths>();
        }

        return bossMoths.GetMothPool();
    }

    private void LoadBoss()
    {
        BossData bossDataScript = FindObjectOfType<BossData>();

        switch (GameData.Instance.Level)
        {
            case LevelProgressionHandler.Levels.Boss1:
                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/HypersonicEventBoss");
                break;
            case LevelProgressionHandler.Levels.Boss2:
                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/KingRockbreath1");
                break;
            case LevelProgressionHandler.Levels.Boss3:
                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/DashEventBoss");
                break;
            case LevelProgressionHandler.Levels.Boss4:
                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/Whalepillar");
                break;
            case LevelProgressionHandler.Levels.Boss5:
                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/KingRockbreath");
                break;
            default:
                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/KingRockbreath");
                break;
        }

        bossDataScript.LoadBoss();
        _gameHud.SetLevelText(GameData.Instance.Level);
    }

    // TODO set this up in the boss script instead
    private void BossEntranceDialogue()
    {
        switch (GameData.Instance.Level)
        {
            case LevelProgressionHandler.Levels.Boss1:
                Toolbox.Tooltips.ShowDialogue("You found the hidden shrine! The key to defeating King Rockbreath can be found here.", 4f);
                break;
            case LevelProgressionHandler.Levels.Boss2:
                if (!GameData.Instance.Data.AbilityData.GetHypersonicStats().AbilityAvailable)
                {
                    Toolbox.Tooltips.ShowDialogue("Without visiting the hidden shrine, you don't stand a chance here! Turn back!", 4f);
                }
                else
                {
                    Toolbox.Tooltips.ShowDialogue("Now that you have unlocked the power of hypersonic, we can defeat King Rockbreath!", 4f);
                }
                break;
        }
    }
}
