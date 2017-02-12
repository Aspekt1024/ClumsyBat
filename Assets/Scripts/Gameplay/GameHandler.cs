using UnityEngine;

public abstract class GameHandler : MonoBehaviour {

    protected PlayerController PlayerController;
    protected Player ThePlayer;
    protected StatsHandler Stats;
    protected GameMusicControl GameMusic;

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
        GameData.Instance.Data.LoadDataObjects();
        PlayerController = FindObjectOfType<PlayerController>();
        ThePlayer = FindObjectOfType<Player>();

        GameObject scriptsObj = GameObject.Find("Scripts");
        GameMusic = scriptsObj.AddComponent<GameMusicControl>();

        Stats = GameData.Instance.Data.Stats;
        EventListener.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        EventListener.OnDeath -= OnDeath;
    }

    public abstract void StartGame();
    public abstract void PauseGame(bool showMenu);
    public abstract void ResumeGame(bool immediate = false);

    public abstract void UpdateGameSpeed(float speed);
    public abstract void LevelComplete();
    
    public virtual void GameOver() { }
    public virtual void TriggerEntered(Collider2D other) { }
    public virtual void TriggerExited(Collider2D other) { }
    public virtual void Collision(Collision2D other) { }
    
    protected virtual void OnDeath() { }

    /// <summary>
    /// Handles the player movement throughout the scene, based
    /// on the distance given by the Player class
    /// </summary>
    public virtual void MovePlayerAtCaveEnd(float dist)
    {
        ThePlayer.transform.position += new Vector3(dist, 0f, 0f);
        AddDistance(dist);
    }
    
    protected virtual void AddDistance(float dist)
    {
        Stats.Distance += dist;
        Stats.TotalDistance += dist;
        if (Stats.Distance > Stats.BestDistance)
        {
            Stats.BestDistance = Stats.Distance;
        }
    }

    public GameStates GetGameState()
    {
        return GameState;
    }
}
