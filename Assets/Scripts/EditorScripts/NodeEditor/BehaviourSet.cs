using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for holding editor/runtime State Machine data
/// </summary>
public abstract class BehaviourSet : ScriptableObject {

    public string BossName;
    public BehaviourSet RootStateMachine;

    public StartAction StartingAction;
    public List<BaseAction> Actions = new List<BaseAction>();
    public bool bEnabled;

    public BaseAction CurrentAction;

    public bool IsType<T>() where T : BehaviourSet
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
