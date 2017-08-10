
public class GameData : Singleton<GameData>
{
    protected GameData() { }

    public DataHandler Data = new DataHandler();

    public enum LevelCompletePaths
    {
        MainPath,
        Secret1,
        Secret2
    }

    public enum AchievementStatus
    {
        Unachieved, Achieved, NewAchievement
    }

    public int NumMoths;
    public bool IsUntouched;
    public bool OnlyOneDamageTaken;
    public AchievementStatus[] Achievements = new AchievementStatus[3];
    public LevelProgressionHandler.Levels Level;
    public bool BossLeftRightTapTutorialSeen;

    private LevelDataContainer.LevelType _levelCompletion;

    public LevelDataContainer.LevelType GetLevelCompletion()
    {
        return _levelCompletion;
    }
    
    public void SetLevelCompletion(LevelCompletePaths path)
    {
        _levelCompletion = new LevelDataContainer.LevelType();
        switch (path)
        {
            case LevelCompletePaths.MainPath:
                _levelCompletion.LevelCompleted = true;
                break;
            case LevelCompletePaths.Secret1:
                _levelCompletion.SecretPath1 = true;
                break;
            case LevelCompletePaths.Secret2:
                _levelCompletion.SecretPath2 = true;
                break;
        }
    }

    public bool IsBossLevel()
    {
        return Level.ToString().Contains("Boss");
    }
}
