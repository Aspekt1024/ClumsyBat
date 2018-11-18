using System;
using System.Collections.Generic;

using Levels = LevelProgressionHandler.Levels;

namespace ClumsyBat.DataManagement
{
    public class LevelDataHandler
    {
        public LevelDataContainer LevelData;
        
        public int NumLevels { get { return Enum.GetNames(typeof(Levels)).Length; } }

        public void SetData(LevelDataContainer data)
        {
            LevelData = data;
            if (LevelData.Levels == null || LevelData.Levels.Count == 0)
            {
                CreateNewLevelData();
            }
        }

        public LevelData GetLevelData(Levels levelID)
        {
            return LevelData.Levels.Find((x) => x.ID == levelID);
        }

        public void ResetData()
        {
            LevelData.Levels.Clear();
            foreach (Levels levelID in Enum.GetValues(typeof(Levels)))
            {
                LevelData.Levels.Add(new LevelData(levelID));
            }
        }

        public void UnlockAllLevels()
        {
            for (int i = 0; i < LevelData.Levels.Count; i++)
            {
                LevelData.Levels[i].LevelUnlocked = true;
            }
        }

        public Levels GetHighestLevel()
        {
            // This ignores secret levels (i.e. this is the main path only)
            Levels level = Levels.Main1;
            while (level < Levels.Boss5 && (GetLevelData(level).LevelUnlocked || level == Levels.Main1))
            {
                Levels nextLevel = LevelProgressionHandler.GetNextLevel(level);
                if (nextLevel == Levels.Unassigned)
                {
                    level = Levels.Main1;
                    break;
                }
                else if (GetLevelData(level).LevelUnlocked)
                {
                    level = nextLevel;
                }
                else
                {
                    break;
                }
            }
            return level;
        }

        public bool IsUnlocked(Levels level) { return GetLevelData(level).LevelUnlocked; }
        public bool IsCompleted(Levels level) { return GetLevelData(level).LevelCompleted; }
        public bool SecretPath1Completed(Levels level) { return GetLevelData(level).SecretPath1; }
        public bool SecretPath2Completed(Levels level) { return GetLevelData(level).SecretPath2; }
        public bool LevelCompletedAchievement(Levels level) { return GetLevelData(level).Achievement1; }
        public bool AllMothsGathered(Levels level) { return GetLevelData(level).Achievement2; }
        public bool NoDamageTaken(Levels level) { return GetLevelData(level).Achievement3; }
        public int GetBestScore(Levels level) { return GetLevelData(level).BestScore; }

        public void SetBestScore(Levels level, int score)
        {
            LevelData levelData = GetLevelData(level);
            levelData.BestScore = score;
        }

        private void CreateNewLevelData()
        {

            Levels[] levels = (Levels[])Enum.GetValues(typeof(Levels));
            LevelData.Levels = new List<LevelData>();

            for (int i = 0; i < levels.Length; i++)
            {
                LevelData.Levels.Add(new LevelData(levels[i]));
            }
        }
    }
}
