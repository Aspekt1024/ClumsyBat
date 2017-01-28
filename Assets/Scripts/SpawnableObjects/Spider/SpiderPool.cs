using UnityEngine;

public sealed class SpiderPool : SpawnPool<SpiderClass> {
    
    public SpiderPool()
    {
        ParentName = "Spiders";
        ParentZ = Toolbox.Instance.ZLayers["Spider"];
        NumObjectsInPool = 5;
        ResourcePath = "Obstacles/Spider";
        ObjTag = "Spider";
        SetupPool();
    }

    public struct SpiderType
    {
        public Spawnable.SpawnType SpawnTransform;
        public bool SpiderSwings;
    }

    public void SetupSpidersInList(SpiderType[] spiderList, float xOffset)
    {
        foreach (SpiderType spider in spiderList)
        {
            SpiderClass newSpider = GetNextObject();
            Spawnable.SpawnType spawnTf = spider.SpawnTransform;
            spawnTf.Pos += new Vector2(xOffset, 0f);
            newSpider.Activate(spawnTf, spider.SpiderSwings);
        }
    }
}
