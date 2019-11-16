using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for holding editor/runtime State Machine data
/// </summary>
public abstract class BehaviourSet : ScriptableObject {
    
    public StateMachine ParentMachine;

    public StartAction StartingAction;
    public List<BaseAction> Actions = new List<BaseAction>();
    public bool IsEnabled;

    public bool IsType<T>() where T : BehaviourSet
    {
        return GetType() == typeof(T);
    }

    public void Stop()
    {
        IsEnabled = false;
        foreach (var action in Actions)
        {
            action.Stop();
        }
    }
    
    public void Restart()
    {
        foreach (var action in Actions)
        {
            action.Stop();
        }
    }

    public abstract void LoopToStart();
    public virtual void Tick(float deltaTime) { }
    public virtual void AddToTickList(BaseAction action) { }
    public virtual void RemoveFromTickList(BaseAction action) { }

}
