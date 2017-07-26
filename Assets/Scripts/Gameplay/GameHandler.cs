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

        GameObject scriptsObj = GameObject.Find("Scripts");
        GameMusic = scriptsObj.AddComponent<GameMusicControl>();

        Stats = GameData.Instance.Data.Stats;

        Stats.MothsEaten = 0;
        GameData.Instance.IsUntouched = true;

        EventListener.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        EventListener.OnDeath -= OnDeath;
    }

    public abstract MothPool GetMothPool();
    public abstract void PauseGame(bool showMenu);
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

    public virtual void Collision(Collision2D other)
    {
        switch (other.collider.tag)
        {
            case "Boss":
                ThePlayer.Die();
                break;
            case "Stalactite":
                Stalactite stal = other.collider.GetComponentInParent<Stalactite>();
                if (stal.Type == SpawnStalAction.StalTypes.Stalactite)
                {
                    stal.Crack();
                    ThePlayer.Die();
                }
                break;
        }
    }

    public GameStates GetGameState()
    {
        return GameState;
    }
}
