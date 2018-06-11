using UnityEngine;

public abstract class GameHandler : MonoBehaviour {

    public GameMusicControl GameMusic;
    
    protected PlayerController PlayerController;
    protected Player ThePlayer;
    protected StatsHandler Stats;

    public enum GameStates
    {
        Starting,
        Normal,
        Paused,
        Resuming,
        PausedForTooltip
    }
    protected GameStates GameState;

    private void Awake()
    {
        Toolbox.Instance.GamePaused = false;
        GameData.Instance.Data.LoadDataObjects();
        PlayerController = FindObjectOfType<PlayerController>();
        ThePlayer = FindObjectOfType<Player>();

        GameObject scriptsObj = GameObject.Find("LevelScripts");
        GameMusic = scriptsObj.AddComponent<GameMusicControl>();

        Stats = GameData.Instance.Data.Stats;

        Stats.MothsEaten = 0;
        GameData.Instance.IsUntouched = true;
        GameData.Instance.OnlyOneDamageTaken = true;

        EventListener.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        EventListener.OnDeath -= OnDeath;
    }

    public abstract MothPool GetMothPool();
    public abstract void PauseGame();
    public abstract void ResumeGame(bool immediate = false);
    
    public abstract void LevelComplete();
    
    public virtual void GameOver() { }
    public virtual void TriggerExited(Collider2D other) { }
    protected virtual void OnDeath() { }

    protected abstract void SetCameraEndPoint();

    public virtual void TriggerEntered(Collider2D other)
    {
        switch (other.tag)
        {
            case "Projectile":
                ThePlayer.Die();
                ThePlayer.GetComponent<Rigidbody2D>().velocity = new Vector2(-3f, 4f);
                break;
            case "Moth":
                Moth moth = other.GetComponentInParent<Moth>();
                moth.ConsumeMoth();
                break;
            case "Boss":
                ThePlayer.Die();
                break;
        }
    }

    // TODO this could be done much better - there's no reason different game modes should give different results for when the player takes damage
    // The only thing this should check for is what state the game is in
    public virtual void Collision(Collision2D other)
    {
        ThePlayer.DeactivateRush();

        switch (other.collider.tag)
        {
            case "Boss":
                DamagePlayer(other.collider.tag);
                break;
            case "Stalactite":
                Stalactite stal = other.collider.GetComponentInParent<Stalactite>();
                if (stal.Type == SpawnStalAction.StalTypes.Stalactite)
                {
                    stal.Crack();
                    DamagePlayer(other.collider.tag);
                }
                break;
        }
    }

    private void DamagePlayer(string tag)
    {
        ThePlayer.PlaySound(ClumsyAudioControl.PlayerSounds.Collision); // TODO sounds
        ThePlayer.TakeDamage(tag);
    }

    public GameStates GetGameState()
    {
        return GameState;
    }
}
