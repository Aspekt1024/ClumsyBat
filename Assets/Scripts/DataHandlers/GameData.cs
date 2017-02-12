
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

    private LevelDataContainer.LevelType _levelCompletion;
    public LevelProgressionHandler.Levels Level;

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
