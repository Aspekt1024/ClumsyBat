using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEditorHandler : BaseObjectHandler
{
    public TriggerEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        parentObj = GetParentTransform("Triggers");
        zLayer = LevelEditorConstants.TriggerZ;
    }

    protected override void Update()
    {

    }

    public override void StoreObjects(ref LevelContainer levelObj)
    {
        level = levelObj;
        var TriggerCounts = GetObjCounts(parentObj);
        for (int i = 0; i < level.Caves.Length; i++)
        {
            level.Caves[i].Triggers = new TriggerHandler.TriggerType [TriggerCounts[i]];
        }

        int[] TriggerNum = new int[level.Caves.Length];
        foreach (Transform Trigger in parentObj)
        {
            int index = GetObjectCaveIndex(Trigger);

            TriggerHandler.TriggerType newTrigger = level.Caves[index].Triggers[TriggerNum[index]];
            newTrigger.SpawnTransform = ProduceSpawnTf(Trigger, index);
            newTrigger.EventId = Trigger.GetComponent<TriggerClass>().EventId;
            newTrigger.EventType = Trigger.GetComponent<TriggerClass>().EventType;
            newTrigger.PausesGame = Trigger.GetComponent<TriggerClass>().PausesGame;
            level.Caves[index].Triggers[TriggerNum[index]] = newTrigger;
            TriggerNum[index]++;
        }
    }

    protected override void SetObjects(LevelContainer level)
    {
        for (int i = 0; i < level.Caves.Length; i++)
        {
            if (level.Caves[i].Triggers == null || level.Caves[i].Triggers.Length == 0) continue;
            foreach (TriggerHandler.TriggerType Trigger in level.Caves[i].Triggers)
            {
                GameObject newTrigger = (GameObject)Object.Instantiate(Resources.Load("Interactables/Trigger"), parentObj);
                Spawnable.SpawnType spawnTf = Trigger.SpawnTransform;
                spawnTf.Pos += new Vector2(i * LevelEditorConstants.TileSizeX, 0f);
                newTrigger.GetComponent<TriggerClass>().SetTransform(newTrigger.transform, spawnTf);
                newTrigger.GetComponent<TriggerClass>().EventId = Trigger.EventId;
                newTrigger.GetComponent<TriggerClass>().EventType = Trigger.EventType;
                newTrigger.GetComponent<TriggerClass>().PausesGame = Trigger.PausesGame;
            }
        }
    }
}
