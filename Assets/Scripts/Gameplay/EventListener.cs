public class EventListener
{
    public delegate void EventHandler();
    public static EventHandler OnDeath;
    public static EventHandler OnLevelWon;
    public static EventHandler OnPauseGame;
    public static EventHandler OnResumeGame;

    public static EventHandler OnMusicToggle;
    public static EventHandler OnSfxToggle;
    public static EventHandler OnTooltipToggle;

    public static EventHandler OnTooltipActioned;

    public static void Death()
    {
        if (OnDeath != null)
            OnDeath();
    }

    public static void LevelWon()
    {
        if (OnLevelWon != null)
            OnLevelWon();
    }

    public static void PauseGame()
    {
        if (OnPauseGame != null)
            OnPauseGame();
    }

    public static void ResumeGame()
    {
        if (OnResumeGame != null)
            OnResumeGame();
    }

    public static void MusicToggle()
    {
        if (OnMusicToggle != null)
            OnMusicToggle();
    }

    public static void SfxToggle()
    {
        if (OnSfxToggle != null)
            OnSfxToggle();
    }

    public static void TooltipToggle()
    {
        if (OnTooltipToggle != null)
            OnTooltipToggle();
    }

    public static void TooltipActioned()
    {
        if (OnTooltipActioned != null)
            OnTooltipActioned();
    }
}
