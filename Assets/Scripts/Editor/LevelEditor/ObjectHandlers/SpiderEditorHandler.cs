using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEditorHandler : BaseObjectHandler
{
    public SpiderEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        resourcePath = "Obstacles/Spider";
        parentObj = GetParentTransform("Spiders");
        zLayer = LevelEditorConstants.SpiderZ;
    }

    protected override void Update()
    {

    }

    public override void StoreObjects(ref LevelContainer levelObj)
    {
        level = levelObj;
        var SpiderCounts = GetObjCounts(parentObj);
        for (int i = 0; i < level.Caves.Length; i++)
        {
            level.Caves[i].Spiders = new SpiderPool.SpiderType[SpiderCounts[i]];
        }

        int[] SpiderNum = new int[level.Caves.Length];
        foreach (Transform Spider in parentObj)
        {
            int index = GetObjectCaveIndex(Spider);

            SpiderPool.SpiderType newSpider = level.Caves[index].Spiders[SpiderNum[index]];
            newSpider.SpawnTransform = ProduceSpawnTf(Spider, index);
            newSpider.SpiderSwings = Spider.GetComponent<SpiderClass>().SwingingSpider;
            level.Caves[index].Spiders[SpiderNum[index]] = newSpider;
            SpiderNum[index]++;
        }
    }

    protected override void SetObjects(LevelContainer level)
    {
        for (int i = 0; i < level.Caves.Length; i++)
        {
            if (level.Caves[i].Spiders == null || level.Caves[i].Spiders.Length == 0) continue;
            foreach (SpiderPool.SpiderType Spider in level.Caves[i].Spiders)
            {
                GameObject newSpider = (GameObject)Object.Instantiate(Resources.Load("Collectibles/Spider"), parentObj);
                Spawnable.SpawnType spawnTf = Spider.SpawnTransform;
                spawnTf.Pos += new Vector2(i * LevelEditorConstants.TileSizeX, 0f);
                newSpider.GetComponent<SpiderClass>().SetTransform(newSpider.transform, spawnTf);
                newSpider.GetComponent<SpiderClass>().SwingingSpider = Spider.SpiderSwings;
            }
        }
    }
}
