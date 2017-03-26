using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObjectHandler
{
    protected LevelEditorObjectHandler objectHandler;
    protected Transform parentObj;
    protected LevelContainer level;
    protected float zLayer;

    public BaseObjectHandler(LevelEditorObjectHandler objHandler)
    {
        objectHandler = objHandler;
    }

    public void GUIUpdate()
    {
        if (parentObj != null)
        {
            parentObj.position = new Vector3(0f, 0f, zLayer);
        }
        Update();
    }

    public void LoadObjects(LevelContainer levelContainer)
    {
        ClearObjects();
        SetObjects(levelContainer);
    }

    private void ClearObjects()
    {
        if (parentObj == null)
        {
            Debug.Log("parent object not found");
            return;
        }

        string parentName = parentObj.name;
        Object.DestroyImmediate(parentObj.gameObject);

        GameObject levelObj = GameObject.Find("Level");
        if (levelObj == null)
            levelObj = new GameObject("Level");

        Transform levelTf = levelObj.transform;
        parentObj = new GameObject(parentName).transform;
        parentObj.SetParent(levelTf);
    }

    public abstract void StoreObjects(ref LevelContainer level);
    protected abstract void SetObjects(LevelContainer level);
    protected abstract void Update();

    protected Transform GetParentTransform(string parentName)
    {
        GameObject parentObj = GameObject.Find(parentName);
        if (parentObj == null)
        {
            GameObject levelObj = GameObject.Find("Level");
            if (levelObj == null)
            {
                levelObj = new GameObject("Level");
            }
            parentObj = new GameObject(parentName);
            parentObj.transform.SetParent(levelObj.transform);
        }
        return parentObj.transform;
    }

    protected int[] GetObjCounts(Transform objParent)
    {
        Debug.Log(objParent);
        int[] objCounts = new int[level.Caves.Length];
        foreach (Transform obj in objParent)
        {
            int index = Mathf.RoundToInt(obj.position.x / LevelEditorConstants.TileSizeX);
            objCounts[index]++;
        }
        return objCounts;
    }

    protected int GetObjectCaveIndex(Transform obj)
    {
        return Mathf.RoundToInt(obj.position.x / LevelEditorConstants.TileSizeX);
    }

    protected Spawnable.SpawnType ProduceSpawnTf(Transform objTf, int index)
    {
        var spawnTf = new Spawnable.SpawnType
        {
            Pos = new Vector2(objTf.position.x - LevelEditorConstants.TileSizeX * index, objTf.position.y),
            Scale = objTf.localScale,
            Rotation = objTf.localRotation
        };
        return spawnTf;
    }

    public bool IsType<T>() where T : BaseObjectHandler
    {
        return GetType().Equals(typeof(T));
    }
}
