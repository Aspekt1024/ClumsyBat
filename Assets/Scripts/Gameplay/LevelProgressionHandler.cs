using UnityEngine;

public static class LevelProgressionHandler
{
    // NB: Level progression will follow this order
    //     When placing levels on the level select, they must follow this pattern!
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

    // Get the next level based on the current path
    public static Levels GetNextLevel(Levels level)
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

    public static Levels GetSecretLevel1(Levels level)
    {
        Levels secretLevel;
        switch (level)
        {
            case Levels.Main3:
                secretLevel = Levels.AltA1;
                break;
            default:
                Debug.Log("Warning: Secret path 1 not set for level " + level);
                secretLevel = level;
                break;
        }
        return secretLevel;
    }

    public static Levels GetSecretLevel2(Levels level)
    {
        Levels secretLevel;
        switch (level)
        {
            case Levels.Main3:
                secretLevel = Levels.AltB1;
                break;
            default:
                Debug.Log("Warning: Secret path 2 not set for level " + level);
                secretLevel = level;
                break;
        }

        return secretLevel;
    }
}
