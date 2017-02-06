public class EventListener
{
    public delegate void EventHandler();
    public static EventHandler OnDeath;
    public static EventHandler OnMusicToggle;
    public static EventHandler OnSfxToggle;

    public static void Death()
    {
        if (OnDeath != null)
            OnDeath();
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
}
