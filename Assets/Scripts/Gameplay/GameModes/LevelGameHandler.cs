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
        StartCoroutine(LevelStartRoutine());
        GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
        SetCameraEndPoint();
        GameStatics.UI.GameHud.SetCurrencyText("0/" + GameStatics.LevelManager.NumMoths);
    }

    protected override void SetCameraEndPoint()
    {
        GameStatics.Camera.SetEndPoint(CaveHandler.GetEndCave().transform.position.x);
    }
    
    private IEnumerator LevelStartRoutine()
    {
        yield return StartCoroutine(CaveEntranceAnimation());

        LevelStart();
        GameStatics.Camera.StartFollowing();
    }

    public IEnumerator CaveEntranceAnimation()
    {
        GameStatics.Player.PossessByAI();

        float timer = 0f;
        const float duration = 1f;
        Vector2 startPos = new Vector2(-Toolbox.TileSizeX / 2, -0.7f);
        Vector2 targetPos = new Vector2(-3, 1.3f);

        GameStatics.Player.SetPlayerPosition(startPos);
        yield return new WaitForSeconds(0.3f); // Allows lantern to settle
        player.Physics.Disable();
        player.Abilities.Perch.Unperch();
        player.Animate(ClumsyAnimator.ClumsyAnimations.Hover);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float animRatio = timer / duration;
            var pos = player.Model.position;
            pos.x = startPos.x - (startPos.x - targetPos.x) * animRatio;
            pos.y = startPos.y - (startPos.y - targetPos.y) * Mathf.Pow(animRatio, 2);
            player.Model.position = pos;
            yield return null;
        }

        player.Animate(ClumsyAnimator.ClumsyAnimations.FlapSlower);
        player.Physics.Enable();
        player.Physics.SetVelocity(6f, 8f);
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
