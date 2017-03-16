using System.Collections.Generic;
using UnityEngine;

public abstract class BossDataContainer : ScriptableObject {

    public string BossName;
    public BossDataContainer RootContainer;

    public StartAction StartingAction;
    public List<BaseAction> Actions = new List<BaseAction>();
    public bool bEnabled;

    public BaseAction CurrentAction;

    public bool IsType<T>() where T : BossDataContainer
    {
        return GetType().Equals(typeof(T));
    }

    public void Stop()
    {
        bEnabled = false;
    }

    public abstract void RequestLoopToStart();
    public virtual void Tick(float deltaTime) { }
    public virtual void AddToTickList(BaseAction action) { }
    public virtual void RemoveFromTickList(BaseAction action) { }
}
