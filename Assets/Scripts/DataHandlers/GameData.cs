
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

    public int NumMoths;
    public bool IsUntouched = true;
    public int NewStars;
    public int TotalStars;
    public LevelProgressionHandler.Levels Level;

    private LevelDataContainer.LevelType _levelCompletion;

    public LevelDataContainer.LevelType GetLevelCompletion()
    {
        return _levelCompletion;
    }
    
    public void SetLevelCompletion(LevelCompletePaths path)
    {
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
}
