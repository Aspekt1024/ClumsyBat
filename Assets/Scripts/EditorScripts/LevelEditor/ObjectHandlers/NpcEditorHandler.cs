using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcEditorHandler : BaseObjectHandler
{
    public NpcEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        parentObj = GetParentTransform("Npcs");
        zLayer = LevelEditorConstants.NpcZ;
    }

    protected override void Update()
    {

    }

    public override void StoreObjects(ref LevelContainer levelObj)
    {
        level = levelObj;
        var NpcCounts = GetObjCounts(parentObj);
        for (int i = 0; i < level.Caves.Length; i++)
        {
            level.Caves[i].Npcs = new NpcPool.NpcType[NpcCounts[i]];
        }

        int[] NpcNum = new int[level.Caves.Length];
        foreach (Transform Npc in parentObj)
        {
            int index = GetObjectCaveIndex(Npc);

            NpcPool.NpcType newNpc = level.Caves[index].Npcs[NpcNum[index]];
            newNpc.SpawnTransform = ProduceSpawnTf(Npc, index);
            newNpc.Type = Npc.GetComponent<NPC>().Type;
            level.Caves[index].Npcs[NpcNum[index]] = newNpc;
            NpcNum[index]++;
        }
    }

    protected override void SetObjects(LevelContainer level)
    {
        for (int i = 0; i < level.Caves.Length; i++)
        {
            if (level.Caves[i].Npcs == null || level.Caves[i].Npcs.Length == 0) continue;
            foreach (NpcPool.NpcType Npc in level.Caves[i].Npcs)
            {
                GameObject newNpc = (GameObject)Object.Instantiate(Resources.Load("NPCs/Nomee"), parentObj);    // TODO update this when there are more NPCs
                Spawnable.SpawnType spawnTf = Npc.SpawnTransform;
                spawnTf.Pos += new Vector2(i * LevelEditorConstants.TileSizeX, 0f);
                newNpc.GetComponent<NPC>().SetTransform(newNpc.transform, spawnTf);
                newNpc.GetComponent<NPC>().Type = Npc.Type;
            }
        }
    }
}
