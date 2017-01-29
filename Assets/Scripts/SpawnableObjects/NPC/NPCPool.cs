using UnityEngine;

public sealed class NPCPool : SpawnPool<NPC> {

    public NPCPool()
    {
        ParentName = "NPCs";
        ParentZ = Toolbox.Instance.ZLayers["NPC"];
        NumObjectsInPool = 1;
        ResourcePath = "NPCs/Nomee";
        ObjTag = "NPC";
        SetupPool();
    }

    public struct NpcType
    {
        public Spawnable.SpawnType SpawnTransform;
        public NPC.NpcTypes Type;
    }

    public void SetupObjectsInList(NpcType[] npcList, float xOffset)
    {
        foreach (NpcType npc in npcList)
        {
            NPC newNpc = GetNextObject();
            Spawnable.SpawnType spawnTf = npc.SpawnTransform;
            spawnTf.Pos += new Vector2(xOffset, 0f);
            newNpc.Activate(spawnTf, npc.Type);
        }
    }
}
