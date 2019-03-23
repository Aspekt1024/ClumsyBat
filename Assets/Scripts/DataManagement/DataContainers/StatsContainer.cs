using System;

namespace ClumsyBat.DataManagement
{
    [Serializable]
    public class StatsContainer : CloneableContainerBase<StatsContainer>
    {
        public const string FILE_NAME = "stats";
        
        public float TotalDistance;
        public float BestDistance;
        public float DashDistance;

        public float DarknessTime;
        public float PlayTime;
        public float IdleTime;
        public float TotalTime;
        
        public int TimesDashed;
        public int ToothDeaths;
        public int RockDeaths;
        public int BossDeaths;
        public int UnknownDeaths;
        public int Deaths;

        public int MostMoths;
        public int TotalMoths;

        public int Highscore;
        public int Currency;
        public int TotalCurrency;
        public int LevelsCompleted;
        public int TotalJumps;
    }
}
