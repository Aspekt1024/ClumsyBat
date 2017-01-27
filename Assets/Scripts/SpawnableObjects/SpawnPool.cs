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

    // TODO put base functions in here if possible
    protected abstract void SetupPool();

    public virtual void PauseGame(bool paused)
    {
        foreach (var obj in ObjPool)
        {
            obj.PauseGame(paused);
        }
    }

    public virtual void SetSpeedX(float speedX)
    {
        foreach (var obj in ObjPool)
        {
            obj.SetSpeed(speedX);
        }
    }
}
