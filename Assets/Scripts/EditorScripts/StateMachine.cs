﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for holding editor/runtime State Machine data
/// </summary>
public abstract class StateMachine : ScriptableObject {

    public string BossName;
    public StateMachine RootStateMachine;

    public StartAction StartingAction;
    public List<BaseAction> Actions = new List<BaseAction>();
    public bool bEnabled;

    public BaseAction CurrentAction;

    public bool IsType<T>() where T : StateMachine
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
