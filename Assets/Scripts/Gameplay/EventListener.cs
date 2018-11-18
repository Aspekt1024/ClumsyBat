using static ClumsyBat.DataManagement.LevelDataHandler;

public class EventListener
{
    public delegate void EventHandler();
    public static EventHandler OnDeath;
    public static EventHandler OnPauseGame;
    public static EventHandler OnResumeGame;

    public static EventHandler OnMusicToggle;
    public static EventHandler OnSfxToggle;
    public static EventHandler OnTooltipToggle;

    public static EventHandler OnTooltipActioned;

    public static EventHandler OnLevelWon;

    /// <summary>
    /// Notifies subscribers that the level has been completed
    /// </summary>
    public static void LevelCompleted()
    {
        OnLevelWon.Invoke();
    }
    
    public static void Death()
    {
        OnDeath.Invoke();
    }

    public static void PauseGame()
    {
        OnPauseGame.Invoke();
    }

    public static void ResumeGame()
    {
        OnResumeGame?.Invoke();
    }

    public static void MusicToggle()
    {
        OnMusicToggle?.Invoke();
    }

    public static void SfxToggle()
    {
        OnSfxToggle?.Invoke();
    }

    public static void TooltipToggle()
    {
        OnTooltipToggle?.Invoke();
    }

    public static void TooltipActioned()
    {
        OnTooltipActioned?.Invoke();
    }
}
