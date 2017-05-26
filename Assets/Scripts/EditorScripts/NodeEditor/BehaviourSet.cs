﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for holding editor/runtime State Machine data
/// </summary>
public abstract class BehaviourSet : ScriptableObject {

    public string Name;
    public StateMachine ParentMachine;

    public StartAction StartingAction;
    public List<BaseAction> Actions = new List<BaseAction>();
    public bool IsEnabled;

    public bool IsType<T>() where T : BehaviourSet
    {
        return GetType().Equals(typeof(T));
    }

    public void Stop()
    {
        IsEnabled = false;
        Actions = new List<BaseAction>();
    }

    public abstract void RequestLoopToStart();
    public virtual void Tick(float deltaTime) { }
    public virtual void AddToTickList(BaseAction action) { }
    public virtual void RemoveFromTickList(BaseAction action) { }

}
