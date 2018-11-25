using UnityEngine;

namespace ClumsyBat.Objects
{
    public sealed class StalPool : SpawnPool<Stalactite>
    {
        public StalPool()
        {
            SetBaseStalParameters();
        }

        public StalPool(int objCount)
        {
            SetBaseStalParameters();
        }

        private void SetBaseStalParameters()
        {
            ParentName = "Stalactites";
            ParentZ = Toolbox.Instance.ZLayers["Stalactite"];
            ResourcePath = "Obstacles/Stalactite";
            ObjTag = "Stalactite";
        }

        public struct StalType
        {
            public Spawnable.SpawnType SpawnTransform;
            public float TriggerPosX;
            public bool DropEnabled;
            public SpawnStalAction.StalTypes Type;
            public float GreenMothChance;
            public float GoldMothChance;
            public float BlueMothChance;
            public int PoolHandlerIndex;
            public SpawnStalAction.StalSpawnDirection Direction;
        }

        public void SetupStalactitesInList(StalType[] stalList, float xOffset)
        {
            foreach (StalType stal in stalList)
            {
                Stalactite newStal = GetObjectFromPool();
                newStal.Spawn(stal, xOffset);
            }
        }
    }
}