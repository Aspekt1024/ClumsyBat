using UnityEngine;
using System.Collections;
using System;
using ClumsyBat;
using ClumsyBat.Players;

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
        player.Model.transform.position = new Vector3(-Toolbox.TileSizeX / 2f, 0f, player.Model.transform.position.z);

        StartCoroutine(LevelStartAnimation());
        GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
        SetCameraEndPoint();
        GameStatics.UI.GameHud.SetCurrencyText("0/" + GameStatics.LevelManager.NumMoths);
    }

    protected override void SetCameraEndPoint()
    {
        GameStatics.Camera.SetEndPoint(CaveHandler.GetEndCave().transform.position.x);
    }
    
    private IEnumerator LevelStartAnimation()
    {

        //player.StartCoroutine(player.CaveEntranceAnimation());

        const float timeToReachDest = 0.6f;
        yield return new WaitForSeconds(timeToReachDest);

        LevelStart();
        GameStatics.Camera.StartFollowing();
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

    public void TriggerEntered(Collider2D other)
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
                    //player.ExitAutoFlightReached();
                }
                break;
            case "Spore":
                //player.Fog.Minimise();
                break;
        }
    }

    public override void LevelComplete(bool viaSecretPath = false)
    {
        Level.LevelWon(viaSecretPath);
    }

    public override void GameOver()
    {
        Level.ShowGameoverMenu();
    }
}
