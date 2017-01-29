using UnityEngine;
using System.Collections.Generic;

public abstract class SpawnPool<T> where T : Spawnable
{
    protected Transform ParentObject;
    protected string ParentName;
    protected float ParentZ;
    protected int NumObjectsInPool;
    protected string ResourcePath;
    public string ObjTag;

    protected readonly List<T> ObjPool = new List<T>();
    private int _index;

    protected T GetNextObject()
    {
        _index++;
        if (_index == NumObjectsInPool)
        {
            _index = 0;
        }
        return ObjPool[_index];
    }
    
    protected virtual void SetupPool()
    {
        CreateParent();
        for (int i = 0; i < NumObjectsInPool; i++)
        {
            CreateObject(i);
        }
    }

    protected void CreateParent()
    {
        ParentObject = new GameObject(ParentName).transform;
        ParentObject.position = new Vector3(0f, 0f, ParentZ);
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

    public virtual void SetSpeedX(float speedX)
    {
        foreach (T obj in ObjPool)
        {
            obj.SetSpeed(speedX);
        }
    }
}
