using UnityEngine;

public sealed class ShroomPool : SpawnPool<Mushroom> {

    public ShroomPool()
    {
        ParentName = "Mushrooms";
        ParentZ = Toolbox.Instance.ZLayers["Mushroom"];
        NumObjectsInPool = 5;
        ResourcePath = "Obstacles/Mushroom";
        ObjTag = "Mushroom";
        SetupPool();
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
            Mushroom mushroom = GetNextObject();
            mushroom.Activate(spawnTf);
        }
    }
}
