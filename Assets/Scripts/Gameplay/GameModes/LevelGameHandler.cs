using UnityEngine;
using System.Collections;
using ClumsyBat;
using ClumsyBat.Players;
using static LevelProgressionHandler;

public sealed class LevelGameHandler : GameHandler
{
    [HideInInspector] public LevelScript Level;
    [HideInInspector] public CaveHandler CaveHandler;

    private float _resumeTimerStart;
    private const float ResumeTimer = 3f;
    private bool caveExitAutoFlightTriggered;

    private Player player;

    private void Start()
    {
        Level = FindObjectOfType<LevelScript>();
        CaveHandler = FindObjectOfType<CaveHandler>();
        player = GameStatics.Player.Clumsy;
    }

    public void StartLevel()
    {
        StartCoroutine(LevelStartRoutine());
        GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
        SetCameraEndPoint();
        GameStatics.UI.GameHud.SetCurrencyText("0/" + GameStatics.LevelManager.NumMoths);
    }

    public void EndLevelMainPath()
    {
        //TODO pause scores
        StartCoroutine(CaveExitRoutine());
    }

    protected override void SetCameraEndPoint()
    {
        GameStatics.Camera.SetEndPoint(CaveHandler.GetEndCave().transform.position.x);
    }
    
    private IEnumerator LevelStartRoutine()
    {
        yield return StartCoroutine(GameStatics.Player.Sequencer.PlaySequence(LevelAnimationSequencer.Sequences.CaveEntrance));

        LevelStart();
        GameStatics.Camera.StartFollowing();
    }

    private IEnumerator CaveExitRoutine()
    {
        yield return StartCoroutine(GameStatics.Player.Sequencer.PlaySequence(LevelAnimationSequencer.Sequences.CaveExit));
        LevelComplete();
    }

    private void LevelStart()
    {
        GameStatics.Player.PossessByPlayer();
        Level.StartGame();
    }

    protected override void OnDeath()
    {
        GetComponent<AudioSource>().Stop();
    }

    public override void LevelComplete(bool viaSecretPath = false)
    {
        Level.LevelWon(viaSecretPath);
        Levels nextLevel = GetNextLevel(GameStatics.LevelManager.Level);
        GameStatics.UI.DropdownMenu.ShowLevelCompletion(GameStatics.LevelManager.Level, nextLevel);
    }

    public override void GameOver()
    {
        Level.ShowGameoverMenu();
    }
}
