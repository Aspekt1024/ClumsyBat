using UnityEngine;

public sealed class MothPool : SpawnPool<Moth>
{
    public MothPool()
    {
        ParentName = "Moths";
        ParentZ = Toolbox.Instance.ZLayers["Moth"];
        NumObjectsInPool = 8;
        ResourcePath = "Collectibles/Moth";
        ObjTag = "Moth";
        SetupPool();
    }

    public struct MothType
    {
        public Spawnable.SpawnType SpawnTransform;
        public Moth.MothColour Colour;
        public MothPathHandler.MothPathTypes PathType;
    }
    
    protected override void SetupPool()
    {
        CreateParent();
        for (int i = 0; i < NumObjectsInPool; i++)
        {
            Moth moth = CreateObject(i);
            moth.PauseAnimation();
        }
    }
    
    public void SetupMothsInList(MothType[] mothList, float xOffset)
    {
        foreach (MothType moth in mothList)
        {
            Moth newMoth = GetNextObject();
            Spawnable.SpawnType spawnTf = moth.SpawnTransform;
            spawnTf.Pos += new Vector2(xOffset, 0f);
            newMoth.Activate(spawnTf, moth.Colour, moth.PathType);
        }
    }

    /// <summary>
    /// Moth activation used in stationary levels e.g. Boss fights
    /// </summary>
    public void ActivateMothInRange(float minY, float maxY, Moth.MothColour colour)
    {
        Moth newMoth = GetNextObject();
        float xPos = 10f + GameObject.FindGameObjectWithTag("MainCamera").transform.position.x;
        var spawnTf = new Spawnable.SpawnType
        {
            Pos = new Vector2(xPos, Random.Range(minY, maxY)),
            Rotation = new Quaternion(),
            Scale = Vector2.one
        };
        newMoth.Activate(spawnTf, colour, MothPathHandler.MothPathTypes.Sine);
    }

    public void ActivateMothFromEssence(Vector2 spawnLoc, Moth.MothColour colour, float despawnTimer)
    {
        Moth newMoth = GetNextObject();
        spawnLoc += new Vector2(GameObject.FindGameObjectWithTag("MainCamera").transform.position.x, 0f);
        var spawnTf = new Spawnable.SpawnType
        {
            Pos = spawnLoc,
            Rotation = new Quaternion(),
            Scale = Vector2.one
        };
        newMoth.Activate(spawnTf, colour, MothPathHandler.MothPathTypes.Clover);
        newMoth.StartCoroutine("SpawnFromEssence", despawnTimer);
    }
}
