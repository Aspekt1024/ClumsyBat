using UnityEngine;

namespace ClumsyBat.Objects
{
    public sealed class ShroomPool : SpawnPool<Mushroom>
    {
        public ShroomPool()
        {
            ParentName = "Mushrooms";
            ParentZ = Toolbox.Instance.ZLayers["Mushroom"];
            ResourcePath = "Obstacles/Mushroom";
            ObjTag = "Mushroom";
        }

        public struct ShroomType
        {
            public Spawnable.SpawnType SpawnTransform;
            public bool SpecialEnabled;
        }

        public void SetupMushroomsInList(ShroomType[] shroomList, float xOffset)
        {
            foreach (ShroomType shroom in shroomList)
            {
                Spawnable.SpawnType spawnTf = shroom.SpawnTransform;
                spawnTf.Pos += new Vector2(xOffset, 0f);
                Mushroom mushroom = GetObjectFromPool();
                mushroom.Spawn(spawnTf);
            }
        }
    }
}