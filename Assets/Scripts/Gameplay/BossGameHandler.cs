using UnityEngine;
using System.Collections;
using ClumsyBat;

using LevelCompletionPaths = ClumsyBat.LevelManagement.LevelCompletionHandler.LevelCompletionPaths;
using ClumsyBat.Players;

//public class BossGameHandlerOLD : GameHandler {

//    public float ClumsySpeed = 6f;
    
//    private Canvas leftRightOverlay;
    
//    private const float ResumeTimerDuration = 3f;
//    private float _resumeTimerStart;

//    private bool startingDialoagueComplete;

//    private const float manualCaveScale = 0.8558578f;

//    private Player player;
    
//    private enum BossGameState
//    {
//        Startup,
//        MovingTowardsBoss,
//        InBossRoom
//    }
//    private BossGameState _state;

//    private void Start()
//    {
//        player = GameStatics.Player.Clumsy;
//        if (GameStatics.LevelManager.Level == LevelProgressionHandler.Levels.Unassigned)
//        {
//            GameStatics.LevelManager.Level = LevelProgressionHandler.Levels.Boss1;
//        }
        
//        // TODO this is never true
//        foreach (Transform tf in GameStatics.Camera.CurrentCamera.transform)
//        {
//            Debug.Log("setup left right overlay");
//            if (tf.name == "LeftRightOverlay")
//            {
//                leftRightOverlay = tf.GetComponent<Canvas>();
//                leftRightOverlay.enabled = false;
//                break;
//            }
//        }

//        LoadBoss();
        
//        //ThePlayer.Fog.Disable();
//        SetCameraEndPoint();
//        StartCoroutine(LoadSequence());
//    }

//	private void Update ()
//    {
//        if (!startingDialoagueComplete)
//        {
//            if (GameStatics.Player.Clumsy.transform.position.x > 0f)
//            {
//                startingDialoagueComplete = true;
//                BossEntranceDialogue();
//            }
//        }

//        if (GameState != GameStates.Normal || _state == BossGameState.InBossRoom) return;
//        if (GameStatics.Player.Clumsy.transform.position.x > Toolbox.TileSizeX * manualCaveScale - 3f)
//        {
//            _state = BossGameState.InBossRoom;
//            StartCoroutine(BossEntrance());
//        }
//	}

//    protected override void SetCameraEndPoint()
//    {
//        GameStatics.Camera.SetEndPoint(Toolbox.TileSizeX * manualCaveScale + 0.8f);
//        GameStatics.Camera.StopFollowingAtEndPoint();
//    }
    
//    private IEnumerator LoadSequence()
//    {
//        yield return new WaitForSeconds(1f);
//        StartGame();
//        yield return new WaitForSeconds(0.8f);
//        //yield return player.StartCoroutine(player.CaveEntranceAnimation());

//        // TODO put this into a function that says "boss level begin" or something
//        GameState = GameStates.Normal;
//        _state = BossGameState.MovingTowardsBoss;
//        player.Physics.SetNormalSpeed();
//        GameStatics.Player.PossessByPlayer();
//    }

//    private IEnumerator BossEntrance()
//    {
//        GameStatics.Player.PossessByAI();
//        GameStatics.Player.AIController.Hover();

//        FindObjectOfType<SlidingDoors>().Close();

//        // TODO boss entrance sequence
//        float timer = 0f;
//        const float duration = 2f;
//        while (timer < duration)
//        {
//            if (!Toolbox.Instance.GamePaused)
//                timer += Time.deltaTime;
//            yield return null;
//        }

//        if (GameStatics.LevelManager.Level == LevelProgressionHandler.Levels.Boss1)
//        {
//            yield return StartCoroutine(ShowMovementTutorial());
//        }

//        GameStatics.Player.AIController.DisableHover();
//        GameStatics.Player.PossessByPlayer();
//        player.Abilities.Flap.MovementMode = FlapComponent.MovementModes.LeftAndRight;
//        BossEvents.BossFightStart();
//    }

//    private IEnumerator ShowMovementTutorial()
//    {
//        if (GameStatics.LevelManager.IsLevelCompleted(LevelProgressionHandler.Levels.Boss1)
//            || GameStatics.Data.EventData.Data.BossLeftRightTapTutorialSeen)
//            yield break;

//        GameStatics.Data.EventData.Data.BossLeftRightTapTutorialSeen = true;
//        leftRightOverlay.enabled = true;
//        GameStatics.UI.GameHud.Hide();

//        yield return new WaitForSeconds(1f);

//        Debug.Log("show movement tutorial is broken");

//        //GameStatics.Player.Clumsy.GetPlayerController().PauseInput(false);
//        //GameStatics.Player.Clumsy.GetPlayerController().WaitForInput();
//        //while (GameStatics.Player.Clumsy.GetPlayerController().WaitingForInput())
//        //{
//        //    yield return null;
//        //}
//        leftRightOverlay.enabled = false;
//        GameStatics.UI.GameHud.Show();
//    }

//    private void StartGame()
//    {
//        GameStatics.UI.GameHud.StartGame();
//        GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
//    }

//    private IEnumerator UpdateResumeTimer()
//    {
//        GameState = GameStates.Resuming;
//        //yield return StartCoroutine(GameStatics.UI.DropdownMenu.Hide);
//        _resumeTimerStart = Time.time;

//        while (GameStatics.Player.Clumsy.State.IsAlive && _resumeTimerStart + ResumeTimerDuration > Time.time)
//        {
//            float timeRemaining = _resumeTimerStart + ResumeTimerDuration - Time.time;
//            GameStatics.UI.GameHud.SetResumeTimer(timeRemaining);
//            yield return null;
//        }
//    }
    
//    public override void LevelComplete(bool viaSecretExit = false)
//    {
//        GameStatics.Player.PossessByAI();
//        GameStatics.Player.AIController.Hover();
//        GameStatics.Player.Clumsy.Physics.DisableCollisions();
//        StartCoroutine(BossFightWon());
//    }

//    private IEnumerator BossFightWon()
//    {
//        if (GameStatics.LevelManager.Level == LevelProgressionHandler.Levels.Boss2)
//        {
//            yield return new WaitForSeconds(2.5f);
//        }
//        else
//        {
//            yield return new WaitForSeconds(1.5f);
//        }

//        GameStatics.LevelManager.LevelCompleted(LevelCompletionPaths.MainPath);

//        // TODO add sound to sound controller script
//    }

//    public override void GameOver()
//    {
//        GameStatics.UI.DropdownMenu.ShowGameOverMenu();
//        GameStatics.UI.GameHud.Hide();
//    }

//    // TODO set up a dictionary
//    private void LoadBoss()
//    {
//        BossData bossDataScript = FindObjectOfType<BossData>();

//        switch (GameStatics.LevelManager.Level)
//        {
//            case LevelProgressionHandler.Levels.BossS1:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/HypersonicEventBoss");
//                break;
//            case LevelProgressionHandler.Levels.BossS2:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/DashEventBoss");
//                break;
//            case LevelProgressionHandler.Levels.Boss1:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/KingRockbreath1");
//                break;
//            case LevelProgressionHandler.Levels.Boss2:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/Rockbreath4");
//                break;
//            case LevelProgressionHandler.Levels.Boss3:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/KingRockbreath2");
//                break;
//            case LevelProgressionHandler.Levels.Boss4:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/Rockbreath5");
//                break;
//            case LevelProgressionHandler.Levels.Boss5:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/Nomee");
//                break;
//            case LevelProgressionHandler.Levels.Boss6:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/RockbreathDemo");
//                break;
//            case LevelProgressionHandler.Levels.Boss7:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/Rockbreath7");
//                break;
//            default:
//                bossDataScript.BossStateMachine = Resources.Load<StateMachine>("NPCs/Bosses/BossBehaviours/KingRockbreath3");
//                break;
//        }

//        bossDataScript.LoadBoss();
//        GameStatics.UI.GameHud.SetLevelText(GameStatics.LevelManager.Level);
//    }

//    // TODO set this up in the boss script instead
//    private void BossEntranceDialogue()
//    {
//        switch (GameStatics.LevelManager.Level)
//        {
//            case LevelProgressionHandler.Levels.BossS1:
//                Toolbox.Tooltips.ShowDialogue("You found the hidden shrine! The key to defeating King Rockbreath can be found here.", 4f);
//                break;
//            case LevelProgressionHandler.Levels.Boss1:
//                if (!GameStatics.Data.Abilities.GetHypersonicStats().AbilityAvailable)
//                {
//                    Toolbox.Tooltips.ShowDialogue("Without visiting the hidden shrine, you don't stand a chance here! Turn back!", 4f);
//                }
//                else
//                {
//                    if (!GameStatics.Data.LevelDataHandler.IsCompleted(LevelProgressionHandler.Levels.Boss2))
//                    {
//                        Toolbox.Tooltips.ShowDialogue("Now that you have unlocked the power of hypersonic, we can defeat King Rockbreath!", 4f);
//                    }
//                }
//                break;
//        }
//    }
//}
