using UnityEngine;

namespace ClumsyBat.Objects
{
    public sealed class NpcPool : SpawnPool<NPC>
    {
        public NpcPool()
        {
            ParentName = "NPCs";
            ParentZ = Toolbox.Instance.ZLayers["NPC"];
            ResourcePath = "NPCs/Nomee";
            ObjTag = "NPC";
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
                NPC newNpc = GetNewObject();
                Spawnable.SpawnType spawnTf = npc.SpawnTransform;
                spawnTf.Pos += new Vector2(xOffset, 0f);
                newNpc.Activate(spawnTf, npc.Type);
            }
        }
    }
}