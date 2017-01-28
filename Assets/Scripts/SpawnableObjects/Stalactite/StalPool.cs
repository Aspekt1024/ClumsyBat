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
        public Spawnable.SpawnType SpawnTranform;
        public Vector2 TriggerPos;
        public bool DropEnabled;
        public bool Flipped;
    }

    public void SetupStalactitesInList(StalType[] stalList, float xOffset)
    {
        foreach (StalType stal in stalList)
        {
            Stalactite newStal = GetNextObject();
            Spawnable.SpawnType spawnTf = stal.SpawnTranform;
            spawnTf.Pos += new Vector2(xOffset, 0f);
            newStal.Activate(spawnTf, stal.DropEnabled, stal.TriggerPos, xOffset);
        }
    }
}
