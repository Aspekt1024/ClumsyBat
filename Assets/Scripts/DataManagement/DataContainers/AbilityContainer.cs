using System;

namespace ClumsyBat.DataContainers
{
    [Serializable]
    public class AbilityContainer
    {
        [Serializable]
        public struct AbilityType
        {
            public int AbilityLevel;
            public int AbilityEvolution;
            public bool AbilityUnlocked;
            public bool AbilityAvailable;
            public bool UpgradeAvailable;
        }

        [Serializable]
        public struct AbilityDataType
        {
            public bool AbilitiesCreated;
            public AbilityType Rush;
            public AbilityType Hypersonic;
            public AbilityType LanternDurationLevel;
            public AbilityType Shield;
            // Lantern view distance
            // Moth Magnet
            // Moth value
        }
        public AbilityDataType Data;
    }
}
