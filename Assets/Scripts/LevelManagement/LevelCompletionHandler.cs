using ClumsyBat.DataManagement;

using Levels = LevelProgressionHandler.Levels;

namespace ClumsyBat.LevelManagement
{
    public class LevelCompletionHandler
    {
        public enum LevelCompletionPaths
        {
            MainPath, Secret1, Secret2
        }

        public void LevelCompleted(Levels level, LevelCompletionPaths path)
        {
            var levelData = GameStatics.Data.LevelDataHandler.GetLevelData(level);

            levelData.LevelCompleted |= path == LevelCompletionPaths.MainPath;
            levelData.SecretPath1 |= path == LevelCompletionPaths.Secret1;
            levelData.SecretPath2 |= path == LevelCompletionPaths.Secret2;
            
            LevelData nextLevel = GetNextLevel(level, path);
            nextLevel?.UnlockLevel();

            GameStatics.Data.Stats.LevelsCompleted++;
            GameStatics.Data.SaveData();

            GameStatics.UI.DropdownMenu.ShowLevelCompletion(level, nextLevel.ID);
        }

        private LevelData GetNextLevel(Levels level, LevelCompletionPaths path)
        {
            switch (path)
            {
                case LevelCompletionPaths.MainPath:
                    return GameStatics.Data.LevelDataHandler.GetLevelData(LevelProgressionHandler.GetNextLevel(level));
                case LevelCompletionPaths.Secret1:
                    return GameStatics.Data.LevelDataHandler.GetLevelData(LevelProgressionHandler.GetSecretLevel1(level));
                case LevelCompletionPaths.Secret2:
                    return GameStatics.Data.LevelDataHandler.GetLevelData(LevelProgressionHandler.GetSecretLevel2(level));
                default:
                    return null;
            }
        }
    }
}
