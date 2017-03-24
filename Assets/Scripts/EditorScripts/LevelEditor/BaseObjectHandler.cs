using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObjectHandler
{
    protected LevelEditorObjectHandler objectHandler;
    protected Transform parentObj;
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
}
