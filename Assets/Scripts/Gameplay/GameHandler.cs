using UnityEngine;

public abstract class GameHandler : MonoBehaviour {

    public GameMusicControl GameMusic;
    
    public enum GameStates
    {
        Starting,
        Normal,
        Paused,
        Resuming,
        PausedForTooltip
    }
    public GameStates GameState { get; set; }

    private void Awake()
    {
        GameObject scriptsObj = GameObject.Find("LevelScripts");
        GameMusic = scriptsObj.AddComponent<GameMusicControl>();
        
        EventListener.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        EventListener.OnDeath -= OnDeath;
    }
    
    public abstract void LevelComplete(bool viaSecretPath = false);
    
    public virtual void GameOver() { }
    public virtual void TriggerExited(Collider2D other) { }
    protected virtual void OnDeath() { }

    protected abstract void SetCameraEndPoint();
}
