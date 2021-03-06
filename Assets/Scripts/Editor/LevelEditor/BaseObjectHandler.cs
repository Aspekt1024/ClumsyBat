﻿using ClumsyBat.Objects;
using UnityEngine;

public abstract class BaseObjectHandler
{
    protected string resourcePath;
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

    public GameObject CreateNewObject()
    {
        if (resourcePath == null)
        {
            Debug.Log("Resource path not set in " + GetType());
            return null;
        }
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(resourcePath), parentObj);
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, zLayer);
        return obj;
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

    public void ShiftRightIfAfterThreshold(float xThreshold)
    {
        foreach (Transform tf in parentObj)
        {
            if (tf.position.x > xThreshold)
                tf.position += Vector3.right * LevelEditorConstants.TileSizeX;
        }
    }

    public void ShiftLeftIfAfterThreshold(float xThreshold)
    {
        foreach (Transform tf in parentObj)
        {
            if (tf.position.x > xThreshold)
                tf.position += Vector3.left * LevelEditorConstants.TileSizeX;
        }
    }
    
    public void DeleteIfWithinRange(float xMin, float xMax)
    {
        for (int i = parentObj.childCount - 1; i >= 0; i--)
        {
            if (parentObj.GetChild(i).position.x > xMin && parentObj.GetChild(i).position.x < xMax)
                Object.DestroyImmediate(parentObj.GetChild(i).gameObject);
        }
    }
}
