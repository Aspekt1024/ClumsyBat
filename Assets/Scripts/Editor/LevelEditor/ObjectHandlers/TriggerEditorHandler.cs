using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TriggerEditorHandler : BaseObjectHandler
{
    public TriggerEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        resourcePath = "Interactables/Trigger";
        parentObj = GetParentTransform("Triggers");
        zLayer = LevelEditorConstants.TriggerZ;
    }

    protected override void Update()
    {
        foreach (Transform trigger in parentObj)
        {
            Vector2 pos = new Vector2(trigger.position.x - trigger.localScale.x / 2, trigger.position.y - trigger.localScale.y / 2);
            Color c = new Color(0.2f, 1f, 0.2f);
            Handles.DrawSolidRectangleWithOutline(new Rect(pos, trigger.localScale), new Color(c.r, c.g, c.b, .2f), new Color(c.r, c.g, c.b, 1f));
        }
    }

    public override void StoreObjects(ref LevelContainer levelObj)
    {
        TriggerEventSerializer.Instance.Save();
        level = levelObj;
        var TriggerCounts = GetObjCounts(parentObj);
        for (int i = 0; i < level.Caves.Length; i++)
        {
            level.Caves[i].Triggers = new TriggerHandler.TriggerType [TriggerCounts[i]];
        }

        int[] TriggerNum = new int[level.Caves.Length];
        foreach (Transform trigger in parentObj)
        {
            int index = GetObjectCaveIndex(trigger);

            TriggerHandler.TriggerType newTrigger = level.Caves[index].Triggers[TriggerNum[index]];
            newTrigger.SpawnTransform = ProduceSpawnTf(trigger, index);
            newTrigger.TrigEvent = trigger.GetComponent<TriggerClass>().TriggerEvent;
            level.Caves[index].Triggers[TriggerNum[index]] = newTrigger;
            TriggerNum[index]++;
        }
    }

    protected override void SetObjects(LevelContainer level)
    {
        TriggerEventSerializer.Instance.Load();
        for (int i = 0; i < level.Caves.Length; i++)
        {
            if (level.Caves[i].Triggers == null || level.Caves[i].Triggers.Length == 0) continue;
            foreach (TriggerHandler.TriggerType Trigger in level.Caves[i].Triggers)
            {
                GameObject newTrigger = (GameObject)Object.Instantiate(Resources.Load("Interactables/Trigger"), parentObj);
                Spawnable.SpawnType spawnTf = Trigger.SpawnTransform;
                spawnTf.Pos += new Vector2(i * LevelEditorConstants.TileSizeX, 0f);
                newTrigger.GetComponent<TriggerClass>().SetTransform(newTrigger.transform, spawnTf);
                newTrigger.GetComponent<TriggerClass>().TriggerEvent.Id = Trigger.TrigEvent.Id;
            }
        }
    }
}
