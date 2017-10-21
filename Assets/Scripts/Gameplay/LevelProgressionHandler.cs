using UnityEngine;

public static class LevelProgressionHandler
{
    // NB: Level progression will follow this order
    //     When placing levels on the level select, they must follow this pattern!
    public enum Levels
    {
        Unassigned = 0,
        Main1 = 1, Main2, Main3, Boss1,
        Main4, Main5, Main6, Boss2,
        Main7, Main8, Main9, Main10, Main11, Boss3,
        Main12, Main13, Main14, Main15, Boss4,
        Main16, Boss5, Boss6, Boss7, Boss8, Boss9,
        BossS1, BossS2, BossS3,
        Endless 
    }

    // Get the next level based on the current path
    public static Levels GetNextLevel(Levels level)
    {
        switch (level)
        {
            case Levels.BossS1:
                return Levels.Main2;    // TODO send back to main menu
            case Levels.BossS2:
                return Levels.Main7;
            default:
                if (level < Levels.Boss5)
                {
                    return level++;
                }
                else
                {
                    return level;
                }
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
