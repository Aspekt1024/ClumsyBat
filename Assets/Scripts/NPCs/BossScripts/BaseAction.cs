﻿using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction
{
    public int ID;
    public List<ActionConnection> connections = new List<ActionConnection>();

    protected StateMachine ParentStateMachine;
    protected BossData bossData;
    protected GameObject boss;
    
    public void Activate()
    {
        ParentStateMachine.CurrentAction = this;
        ActivateBehaviour();
    }

    public abstract void ActivateBehaviour();

    public virtual void Tick(float deltaTime) { }

    public void CallNext(int id)
    {
        foreach (var conn in connections)
        {
            if (conn.ID == id)
                conn.CallNext();
        }
    }

    public BaseAction GetNextAction(int id)
    {
        BaseAction nextAction = null;
        foreach (var conn in connections)
        {
            if (conn.ID == id && conn.IsConnected())
            {
                nextAction = conn.ConnectedInterface.Action;
                break;
            }
        }
        return nextAction;
    }

    protected ActionConnection GetInterface(int index)
    {
        ActionConnection iface = new ActionConnection();
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].ID == index)
            {
                iface = connections[i];
                break;
            }
        }
        return iface;
    }

    public virtual GameObject GetObject(int id) { return null; }

    public virtual void GameSetup(StateMachine parentMachine, BossData bossData, GameObject bossReference)
    {
        ParentStateMachine = parentMachine;
        this.bossData = bossData;
        boss = bossReference;
    }

    public bool IsType<T>() where T : BaseAction
    {
        return GetType().Equals(typeof(T));
    }
}
