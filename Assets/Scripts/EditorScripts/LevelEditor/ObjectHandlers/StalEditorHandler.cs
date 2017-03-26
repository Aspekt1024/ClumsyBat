using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalEditorHandler : BaseObjectHandler
{
    private StalactiteEditor stalHandler;

    public StalEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        parentObj = GetParentTransform("Stalactites");
        zLayer = LevelEditorConstants.StalactiteZ;
        stalHandler = new StalactiteEditor(parentObj);
    }

    protected override void Update()
    {
        // TODO show trigger
        // NB trigger is relative to the position of the stalactite (and would be negative pretty much every time)
        //stalHandler.ProcessStalactites();
    }

    public override void StoreObjects(ref LevelContainer level)
    {
        var StalCounts = GetObjCounts(parentObj);
        for (int i = 0; i < level.Caves.Length; i++)
        {
            level.Caves[i].Stals = new StalPool.StalType[StalCounts[i]];
        }

        int[] StalNum = new int[level.Caves.Length];
        foreach (Transform Stal in parentObj)
        {
            int index = GetObjectCaveIndex(Stal);

            StalPool.StalType newStal = level.Caves[index].Stals[StalNum[index]];
            newStal.SpawnTransform = ProduceSpawnTf(Stal, index);
            newStal.DropEnabled = Stal.GetComponent<Stalactite>().DropEnabled;
            newStal.TriggerPosX = Stal.GetComponent<Stalactite>().TriggerPosX;
            level.Caves[index].Stals[StalNum[index]] = newStal;
            StalNum[index]++;
        }
    }
    
    protected override void SetObjects(LevelContainer level)
    {
        for (int i = 0; i < level.Caves.Length; i++)
        {
            if (level.Caves[i].Stals == null || level.Caves[i].Stals.Length == 0) continue;
            foreach (StalPool.StalType Stal in level.Caves[i].Stals)
            {
                GameObject newStal = (GameObject)Object.Instantiate(Resources.Load("Obstacles/Stalactite"), parentObj);
                Spawnable.SpawnType spawnTf = Stal.SpawnTransform;
                spawnTf.Pos += new Vector2(i * LevelEditorConstants.TileSizeX, 0f);
                newStal.GetComponent<Stalactite>().SetTransform(newStal.transform, spawnTf);
                newStal.GetComponent<Stalactite>().DropEnabled = Stal.DropEnabled;
                newStal.GetComponent<Stalactite>().TriggerPosX = Stal.TriggerPosX;
            }
        }
    }
}
