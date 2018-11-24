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
        StartCoroutine(CaveExitAnimation());
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

    private IEnumerator CaveEntranceAnimation()
    {
        GameStatics.Player.PossessByAI();
        
        float timer = 0f;
        const float duration = 1f;
        Vector2 targetPos = new Vector2(-3, 1.3f);

        Vector2 startPos = new Vector2(-Toolbox.TileSizeX / 2, -0.7f);
        GameStatics.Player.SetPlayerPosition(startPos);
        yield return new WaitForSeconds(0.3f); // Allows lantern to settle

        player.Physics.Disable();
        player.Abilities.Perch.Unperch();
        player.Animate(ClumsyAnimator.ClumsyAnimations.Hover);
        player.Fog.StartOfLevel();

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

    private IEnumerator CaveExitAnimation()
    {
        GameStatics.Player.PossessByAI();

        player.Physics.Disable();
        player.Abilities.Perch.Unperch();

        float animTimer = 0f;
        const float animDuration = 0.9f;
        Vector3 originalPos = player.Model.position;
        Vector3 targetExitPoint = new Vector3(player.Model.position.x + Toolbox.TileSizeX / 2f, -0.5f, originalPos.z);

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            float animRatio = animTimer / animDuration;
            player.Model.position = Vector3.Lerp(originalPos, targetExitPoint, animRatio);
            yield return null;
        }
        player.Lantern.transform.position += new Vector3(.3f, 0f, 0f);
        player.Fog.EndOfLevel();
        player.Physics.Body.velocity = Vector2.zero;

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
