using UnityEngine;

public static class LevelProgressionHandler
{
    // NB: Level progression will follow this order
    //     When placing levels on the level select, they must follow this pattern!
    public enum Levels
    {
        Unassigned = 0,
        Main1 = 1, Main2 = 2, Main3 = 3, Boss1 = 4,
        Main4, Main5, Main6, Boss2,
        Main7, Main8, Boss3,
        Main9, Main10, Main11, Boss4,
        BossS1, BossS2,
        Credits
    }

    // Get the next level based on the current path
    public static Levels GetNextLevel(Levels level)
    {
        // Boss4 is the last level. If we get here, point to credits
        if (level == Levels.Boss4)
        {
            return Levels.Credits;
        }
        
        switch (level)
        {
            case Levels.BossS1:
                return Levels.Unassigned;
            case Levels.BossS2:
                return Levels.Unassigned;
            case Levels.Boss4:
                return Levels.Unassigned;
        }

        if (level < Levels.Boss4)
        {
            return (Levels)(((int)level) + 1);
        }
        else
        {
            return level;
        }
    }

    public static Levels GetSecretLevel1(Levels level)
    {
        Levels secretLevel;
        switch (level)
        {
            case Levels.Main2:
                secretLevel = Levels.BossS1;
                break;
            case Levels.Main7:
                secretLevel = Levels.BossS2;
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
        return Levels.Unassigned;
    }
}
