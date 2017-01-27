using UnityEditor;
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

    // TODO move some of this to the base
    protected override void SetupPool()
    {
        ParentObject = new GameObject(ParentName).transform;
        ParentObject.position = new Vector3(0f, 0f, ParentZ);
        for (int i = 0; i < NumObjectsInPool; i++)
        {
            GameObject mothObj = (GameObject)Object.Instantiate(Resources.Load(ResourcePath), ParentObject);
            Moth moth = mothObj.GetComponent<Moth>();
            mothObj.name = ObjTag + i;
            mothObj.transform.position = Toolbox.Instance.HoldingArea;
            moth.PauseAnimation();
            ObjPool.Add(moth);
        }
    }

    // TODO move some of this to the base
    public void SetupMothsInList(MothType[] mothList, float xOffset)
    {
        foreach (MothType moth in mothList)
        {
            Moth newMoth = GetNextObject();
            Spawnable.SpawnType spawnTF = moth.SpawnTransform;
            spawnTF.Pos += new Vector2(xOffset, 0f);
            newMoth.Activate(spawnTF, moth.Colour, moth.PathType);
        }
    }

    /// <summary>
    /// Moth activation used in stationary levels e.g. Boss fights
    /// </summary>
    public void ActivateMothInRange(float minY, float maxY, Moth.MothColour colour)
    {
        Moth newMoth = GetNextObject();
        var spawnTf = new Spawnable.SpawnType
        {
            Pos = new Vector2(10f, Random.Range(minY, maxY)),
            Rotation = new Quaternion(),
            Scale = Vector2.one
        };
        newMoth.Activate(spawnTf, colour, MothPathHandler.MothPathTypes.Sine);
    }
}
