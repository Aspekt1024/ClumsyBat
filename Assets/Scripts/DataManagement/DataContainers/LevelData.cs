using System;

using Levels = LevelProgressionHandler.Levels;

namespace ClumsyBat.DataManagement
{
    [Serializable]
    public class LevelData
    {
        public Levels ID;
        public bool LevelCompleted;
        public bool LevelUnlocked;
        public bool SecretPath1;
        public bool SecretPath2;
        public bool Achievement1;
        public bool Achievement2;
        public bool Achievement3;
        public int BestScore;

        public LevelData(Levels id)
        {
            ID = id;
            LevelCompleted = false;
            LevelUnlocked = false;
            SecretPath1 = false;
            SecretPath2 = false;
            Achievement1 = false;
            Achievement2 = false;
            Achievement3 = false;
            BestScore = 0;
        }

        public void UnlockLevel()
        {
            LevelUnlocked = true;
        }
    }
}
