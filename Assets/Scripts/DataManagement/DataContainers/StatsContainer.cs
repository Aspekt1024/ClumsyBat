using System;

namespace ClumsyBat.DataManagement
{
    [Serializable]
    public class StatsContainer : CloneableContainerBase<StatsContainer>
    {
        public const string FILE_NAME = "stats";
        
        public float TotalDistance;
        public float DashDistance;

        public float DarknessTime;
        public float PlayTime;
        public float IdleTime;

        public int HypersonicCount;
        public int TimesDashed;
        public int ShieldUses;
        public int Perches;

        public int DamageTaken;
        public int ToothDeaths;
        public int SpiderDeaths;
        public int BossDeaths;
        public int UnknownDeaths;
        public int Deaths;

        public int TotalMoths;

        public int LevelsCompleted;
        public int BossesDefeated;
        public int TotalJumps;
    }
}
