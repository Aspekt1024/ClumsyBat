using UnityEngine;
using System.Collections;

public abstract class GameHandler : MonoBehaviour {

    protected PlayerController PlayerController;
    protected Player ThePlayer;

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
        PlayerController = FindObjectOfType<PlayerController>();
        ThePlayer = FindObjectOfType<Player>();
    }

    public abstract void StartGame();
    public abstract void PauseGame(bool showMenu);
    public abstract void ResumeGame(bool immediate = false);

    public abstract void UpdateGameSpeed(float speed);
    public abstract void LevelComplete();
    
    public virtual void Death() { }
    public virtual void GameOver() { }
    public virtual void TriggerEntered(Collider2D other) { }
    public virtual void Collision(Collision2D other) { }

    public GameStates GetGameState()
    {
        return GameState;
    }

}
