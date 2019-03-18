using UnityEngine;
using System.Collections;
using ClumsyBat;

using LevelCompletionPaths = ClumsyBat.LevelManagement.LevelCompletionHandler.LevelCompletionPaths;
using ClumsyBat.Players;
using System.Collections.Generic;

public class BossHandler : MonoBehaviour {

    private readonly Dictionary<LevelProgressionHandler.Levels, string> bossStateMachinesDict = new Dictionary<LevelProgressionHandler.Levels, string>()
    {
        {LevelProgressionHandler.Levels.BossS1, "NPCs/Bosses/BossBehaviours/HypersonicEventBoss"},
        {LevelProgressionHandler.Levels.BossS2, "NPCs/Bosses/BossBehaviours/DashEventBoss"},
        {LevelProgressionHandler.Levels.Boss1, "NPCs/Bosses/BossBehaviours/KingRockbreath1"},
        {LevelProgressionHandler.Levels.Boss2, "NPCs/Bosses/BossBehaviours/Rockbreath4"},
        {LevelProgressionHandler.Levels.Boss3, "NPCs/Bosses/BossBehaviours/KingRockbreath2"},
        {LevelProgressionHandler.Levels.Boss4, "NPCs/Bosses/BossBehaviours/Rockbreath5"},
        {LevelProgressionHandler.Levels.Boss5, "NPCs/Bosses/BossBehaviours/Nomee"},
        {LevelProgressionHandler.Levels.Boss6, "NPCs/Bosses/BossBehaviours/RockbreathDemo"},
        {LevelProgressionHandler.Levels.Boss7, "NPCs/Bosses/BossBehaviours/Rockbreath7"},
        {LevelProgressionHandler.Levels.Boss8, "NPCs/Bosses/BossBehaviours/KingRockbreath3"},
    };

    private const float ResumeTimerDuration = 3f;
    private float _resumeTimerStart;

    private bool startingDialoagueComplete;

    private const float manualCaveScale = 0.8558578f;

    private Player player;

    private enum BossGameState
    {
        Startup,
        MovingTowardsBoss,
        InBossRoom
    }
    private BossGameState _state;
    
    public void LoadBoss()
    {
        BossData bossDataScript = FindObjectOfType<BossData>();
        string stateMachineResourcePath = bossStateMachinesDict[GameStatics.LevelManager.Level];
        bossDataScript.BossStateMachine = Resources.Load<StateMachine>(stateMachineResourcePath);
        bossDataScript.LoadBoss();
        GameStatics.UI.GameHud.SetLevelText(GameStatics.LevelManager.Level);

        GameStatics.Player.Clumsy.Fog.Disable();
        SetCameraEndPoint();

        _state = BossGameState.MovingTowardsBoss;
        player.Physics.SetNormalSpeed();
        GameStatics.Player.PossessByPlayer();
    }

    public void LevelComplete(bool viaSecretExit = false)
    {
        GameStatics.Player.PossessByAI();
        GameStatics.Player.AIController.Hover();
        GameStatics.Player.Clumsy.Physics.DisableCollisions();
        StartCoroutine(BossFightWon());
    }

    private void Start()
    {
        player = GameStatics.Player.Clumsy;
        if (GameStatics.LevelManager.Level == LevelProgressionHandler.Levels.Unassigned)
        {
            GameStatics.LevelManager.Level = LevelProgressionHandler.Levels.Boss1;
        }

        // TODO this is never true
        foreach (Transform tf in GameStatics.Camera.CurrentCamera.transform)
        {
            Debug.Log("setup left right overlay");
            if (tf.name == "LeftRightOverlay")
            {
                leftRightOverlay = tf.GetComponent<Canvas>();
                leftRightOverlay.enabled = false;
                break;
            }
        }
    }

    private void Update()
    {
        if (!startingDialoagueComplete)
        {
            Debug.Log("boss entrance dialogue not complete. implement me");
            return;
            if (GameStatics.Player.Clumsy.Model.position.x > 0f)
            {
                startingDialoagueComplete = true;
                BossEntranceDialogue();
            }
        }

        if (_state == BossGameState.InBossRoom) return;
        if (GameStatics.Player.Clumsy.Model.position.x > Toolbox.TileSizeX * manualCaveScale - 3f)
        {
            _state = BossGameState.InBossRoom;
            StartCoroutine(BossEntrance());
        }
    }

    private void SetCameraEndPoint()
    {
        GameStatics.Camera.SetEndPoint(Toolbox.TileSizeX * manualCaveScale + 0.8f);
        GameStatics.Camera.StopFollowingAtEndPoint();
    }
    
    private IEnumerator BossEntrance()
    {
        GameStatics.Player.PossessByAI();
        GameStatics.Player.AIController.Hover();

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

        if (GameStatics.LevelManager.Level == LevelProgressionHandler.Levels.Boss1)
        {
            yield return StartCoroutine(ShowMovementTutorial());
        }

        GameStatics.Player.AIController.DisableHover();
        GameStatics.Player.PossessByPlayer();
        player.Abilities.Flap.MovementMode = FlapComponent.MovementModes.LeftAndRight;
        BossEvents.BossFightStart();
    }


    private Canvas leftRightOverlay;

    private IEnumerator ShowMovementTutorial()
    {
        Debug.Log("accessing the show movement tutorial. fix this!");
        yield break;
        if (GameStatics.LevelManager.IsLevelCompleted(LevelProgressionHandler.Levels.Boss1)
            || GameStatics.Data.EventData.Data.BossLeftRightTapTutorialSeen)
            yield break;

        GameStatics.Data.EventData.Data.BossLeftRightTapTutorialSeen = true;
        leftRightOverlay.enabled = true;
        GameStatics.UI.GameHud.Hide();

        yield return new WaitForSeconds(1f);

        Debug.Log("show movement tutorial is broken");

        //GameStatics.Player.Clumsy.GetPlayerController().PauseInput(false);
        //GameStatics.Player.Clumsy.GetPlayerController().WaitForInput();
        //while (GameStatics.Player.Clumsy.GetPlayerController().WaitingForInput())
        //{
        //    yield return null;
        //}
        leftRightOverlay.enabled = false;
        GameStatics.UI.GameHud.Show();
    }

    private IEnumerator BossFightWon()
    {
        if (GameStatics.LevelManager.Level == LevelProgressionHandler.Levels.Boss2)
        {
            yield return new WaitForSeconds(2.5f);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }

        GameStatics.LevelManager.LevelCompleted(LevelCompletionPaths.MainPath);

        // TODO add sound to sound controller script
    }

    // TODO set this up in the boss script instead
    private void BossEntranceDialogue()
    {
        string dialogue = "";
        switch (GameStatics.LevelManager.Level)
        {
            case LevelProgressionHandler.Levels.BossS1:
                dialogue = "You found the hidden shrine! The key to defeating King Rockbreath can be found here.";
                break;
            case LevelProgressionHandler.Levels.Boss1:
                if (!GameStatics.Data.Abilities.GetHypersonicStats().AbilityAvailable)
                {
                    dialogue = "Without visiting the hidden shrine, you don't stand a chance here! Turn back!";
                }
                else
                {
                    if (!GameStatics.Data.LevelDataHandler.IsCompleted(LevelProgressionHandler.Levels.Boss2))
                    {
                        dialogue = "Now that you have unlocked the power of hypersonic, we can defeat King Rockbreath!";
                    }
                }
                break;
            default:
                return;
        }
        Toolbox.Tooltips.ShowDialogue(new TriggerEvent() { Dialogue = new List<string>() { dialogue } });
    }
}
