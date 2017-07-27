using UnityEngine;
using System.Collections.Generic;

public abstract class SpawnPool<T> where T : Spawnable
{
    public string ObjTag;

    protected Transform ParentObject;
    protected string ParentName;
    protected float ParentZ;
    protected string ResourcePath;
    protected int numObjectsInPool;
    
    protected readonly List<T> ObjPool = new List<T>();
    private int index;
    private int numObjects;

    protected T GetNewObject()
    {
        if (ParentObject == null)
            CreateParent();

        numObjects++;
        return CreateObject(numObjects);
    }

    protected void CreateParent()
    {
        ParentObject = new GameObject(ParentName).transform;
        ParentObject.position = new Vector3(0f, 0f, ParentZ);
    }

    protected void SetupPool(int objCount)
    {
        for (int i = 0; i < objCount; i++)
        {
            CreateObject(i);
        }
    }

    protected T GetNextObj()
    {
        while(ObjPool[index].IsActive)
        {
            index++;
        }
        return ObjPool[index];
    }

    protected T CreateObject(int objNum)
    {
        var newObj = (GameObject)Object.Instantiate(Resources.Load(ResourcePath), ParentObject);
        newObj.name = ObjTag + objNum;
        newObj.transform.position = Toolbox.Instance.HoldingArea;
        var objScript = newObj.GetComponent<T>();
        ObjPool.Add(objScript);
        return objScript;
    }

    public virtual void PauseGame(bool paused)
    {
        foreach (var obj in ObjPool)
        {
            obj.PauseGame(paused);
        }
    }
}
