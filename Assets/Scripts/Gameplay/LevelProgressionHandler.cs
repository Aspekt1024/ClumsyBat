using UnityEngine;

public class LevelProgressionHandler
{
    private readonly LevelDataControl _levelDataParent;

    public enum Levels
    {
        Unassigned,
        Main1 = 1, Main2, Boss1,
        Main3, Main4, Main5, Main6, Main7, Main8, Boss2,
        Main9, Main10, Main11, Main12, Boss3,
        Main13, Main14, Main15, Boss4,
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
        
        Debug.Log(level + "completed");
        _levelDataParent.SetCompleted(level);

        if (levelCompletion.SecretPath1)
        {
            UnlockSecretPath1(level);
        }
        else if (levelCompletion.SecretPath2)
        {
            UnlockSecretPath2(level);
        }
        else if (levelCompletion.LevelCompleted)
        {
            level = GetNextLevel(level);
            Debug.Log("unlocking " + level);
            _levelDataParent.UnlockLevel(level);
        }

        // TODO update StatsHandler to exist in GameData.
        // TODO should make things interesting
        //_stats.LevelsCompleted++;
        //_stats.SaveStats();
    }

    // Get the next level based on the current path
    private Levels GetNextLevel(Levels level)
    {
        if (level < Levels.Boss4 
            || level.ToString().Contains("AltA") && level < Levels.AltA3
            || level.ToString().Contains("AltB") && level < Levels.AltB2
            || level.ToString().Contains("AltC") && level < Levels.AltC1
            || level.ToString().Contains("AltD") && level < Levels.AltD4)
        {
            level++;
        }
        return level;
    }

    private void UnlockSecretPath1(Levels level)
    {
        switch (level)
        {
            case Levels.Main3:
                _levelDataParent.UnlockLevel(Levels.AltA1);
                break;
        }
    }

    private void UnlockSecretPath2(Levels level)
    {
        switch (level)
        {
            case Levels.Main3:
                _levelDataParent.UnlockLevel(Levels.AltB1);
                break;
        }
    }
}
