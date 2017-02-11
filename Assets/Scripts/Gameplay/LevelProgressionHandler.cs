
public class LevelProgressionHandler
{
    private readonly LevelDataControl _levelDataParent;

    public enum Levels
    {
        Unassigned,
        Main1 = 1, Main2, Boss1,
        Main3, Main4, Main5, Main6, Main7, Main8, Boss2,
        Main9, Main10, Main11, Main12, Main13, Main14, Main15,
        AltA1, AltA2, AltA3,
        AltB1, AltB2,
        AltC1,
        AltD1, AltD2, AltD3, AltD4,
        Training1, Training2,
        Endless
    }


	public LevelProgressionHandler(LevelDataControl parent)
	{
	    _levelDataParent = parent;
        EventListener.OnLevelWon += OnLevelWon;
    }

    ~LevelProgressionHandler()
    {
        EventListener.OnLevelWon -= OnLevelWon;
    }

    private void OnLevelWon()
    {
        var levelCompletion = GameData.Instance.GetLevelCompletion();
        var level = GameData.Instance.Level;

        switch (level)
        {
            // TODO define which secret paths point where
            case Levels.Main3:
                if (levelCompletion.SecretPath1)
                {
                    _levelDataParent.UnlockLevel(Levels.AltA1);
                }
                break;
        }

        // TODO update StatsHandler to exist in GameData.
        // TODO should make things interesting
        //_stats.LevelsCompleted++;
        //_stats.SaveStats();
        _levelDataParent.SetCompleted(level);
        _levelDataParent.UnlockLevel(level);
    }
}
