using UnityEngine;

public sealed class StalPool : SpawnPool<Stalactite> {

    public StalPool()
    {
        ParentName = "Stalactites";
        ParentZ = Toolbox.Instance.ZLayers["Stalactite"];
        NumObjectsInPool = 25;
        ResourcePath = "Obstacles/Stalactite";
        ObjTag = "Stalactite";
        SetupPool();
    }

    public struct StalType
    {
        public Spawnable.SpawnType SpawnTransform;
        public float TriggerPosX;
        public bool DropEnabled;
        public SpawnStalAction.StalTypes Type;
    }

    public void SetupStalactitesInList(StalType[] stalList, float xOffset)
    {
        foreach (StalType stal in stalList)
        {
            Stalactite newStal = GetNextObject();
            newStal.Activate(stal, xOffset);
        }
    }
}
