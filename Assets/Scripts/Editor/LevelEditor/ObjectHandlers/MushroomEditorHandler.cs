using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomEditorHandler : BaseObjectHandler
{
    public MushroomEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        parentObj = GetParentTransform("Mushrooms");
        zLayer = LevelEditorConstants.MushroomZ;
    }

    protected override void Update()
    {

    }

    public override void StoreObjects(ref LevelContainer levelObj)
    {
        level = levelObj;
        var MushroomCounts = GetObjCounts(parentObj);
        for (int i = 0; i < level.Caves.Length; i++)
        {
            level.Caves[i].Shrooms = new ShroomPool.ShroomType[MushroomCounts[i]];
        }

        int[] MushroomNum = new int[level.Caves.Length];
        foreach (Transform Mushroom in parentObj)
        {
            int index = GetObjectCaveIndex(Mushroom);

            ShroomPool.ShroomType newMushroom = level.Caves[index].Shrooms[MushroomNum[index]];
            newMushroom.SpawnTransform = ProduceSpawnTf(Mushroom, index);
            //newMushroom.SpecialEnabled = Mushroom.GetComponent<Mushroom>(). undefined
            level.Caves[index].Shrooms[MushroomNum[index]] = newMushroom;
            MushroomNum[index]++;
        }
    }

    protected override void SetObjects(LevelContainer level)
    {
        for (int i = 0; i < level.Caves.Length; i++)
        {
            if (level.Caves[i].Shrooms == null || level.Caves[i].Shrooms.Length == 0) continue;
            foreach (ShroomPool.ShroomType Mushroom in level.Caves[i].Shrooms)
            {
                GameObject newMushroom = (GameObject)Object.Instantiate(Resources.Load("Collectibles/Mushroom"), parentObj);
                Spawnable.SpawnType spawnTf = Mushroom.SpawnTransform;
                spawnTf.Pos += new Vector2(i * LevelEditorConstants.TileSizeX, 0f);
                newMushroom.GetComponent<Mushroom>().SetTransform(newMushroom.transform, spawnTf);
            }
        }
    }
}
