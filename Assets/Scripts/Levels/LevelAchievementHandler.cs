using ClumsyBat.DataManagement;

namespace ClumsyBat.LevelManagement
{
    public static class LevelAchievementHandler
    {
        public enum AchievementStatus
        {
            Unachieved, Achieved, NewAchievement
        }

        public static AchievementStatus[] GetLevelAchievements()
        {
            GameData data = GameStatics.Data.GameState;
            var levelData = GameStatics.Data.LevelDataHandler.GetLevelData(GameStatics.LevelManager.Level);

            AchievementStatus[] achievements = new AchievementStatus[3]
            {
                levelData.Achievement1 ? AchievementStatus.Achieved : AchievementStatus.Unachieved,
                levelData.Achievement2 ? AchievementStatus.Achieved : AchievementStatus.Unachieved,
                levelData.Achievement3 ? AchievementStatus.Achieved : AchievementStatus.Unachieved
            };

            if (GameStatics.LevelManager.IsBossLevel)
            {
                // Boss achievements
                levelData.Achievement1 |= levelData.LevelCompleted;
                levelData.Achievement2 |= data.NumTimesTakenDamage <= 1;
                levelData.Achievement3 |= data.NumTimesTakenDamage == 0;
            }
            else
            {
                // Standard level achievements
                levelData.Achievement1 |= data.MothsEaten == GameStatics.LevelManager.NumMoths;
                levelData.Achievement2 |= data.NumTimesTakenDamage == 0;
                levelData.Achievement3 |= data.Score >= GameStatics.LevelManager.ScoreToBeat;
            }

            if (levelData.Achievement1 && achievements[0] == AchievementStatus.Unachieved) achievements[0] = AchievementStatus.NewAchievement;
            if (levelData.Achievement2 && achievements[1] == AchievementStatus.Unachieved) achievements[1] = AchievementStatus.NewAchievement;
            if (levelData.Achievement3 && achievements[2] == AchievementStatus.Unachieved) achievements[2] = AchievementStatus.NewAchievement;

            return achievements;
        }
    }
}
