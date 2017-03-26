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
    }

    public void SetupStalactitesInList(StalType[] stalList, float xOffset)
    {
        foreach (StalType stal in stalList)
        {
            Stalactite newStal = GetNextObject();
            Spawnable.SpawnType spawnTf = stal.SpawnTransform;
            spawnTf.Pos += new Vector2(xOffset, 0f);
            newStal.Activate(spawnTf, stal.DropEnabled, stal.TriggerPosX, xOffset);
        }
    }
}
