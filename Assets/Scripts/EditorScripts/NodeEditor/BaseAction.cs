using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public abstract class BaseAction
{
    public int ID;
    public List<ActionConnection> connections = new List<ActionConnection>();

    [XmlIgnore] public bool IsStopped;
    [XmlIgnore] public bool IsActive;
    [XmlIgnore] public bool IsNewActivation;

    protected BehaviourSet behaviourSet;
    protected BossData bossData;
    protected GameObject boss;
    
    public void Activate()
    {
        if (behaviourSet.IsType<State>() && !((State)behaviourSet).IsEnabled) return;
        if (IsType<StateAction>()) ((StateAction)this).State.IsEnabled = true;
        
        IsActive = true;
        IsNewActivation = true;
        IsStopped = false;
        ActivateBehaviour();
    }

    protected abstract void ActivateBehaviour();

    public virtual void Stop()
    {
        IsActive = false;
        IsStopped = true;
    }

    public virtual void Tick(float deltaTime) { }

    public void ForceCallNext(int id)
    {
        foreach(var conn in connections)
        {
            if (conn.ID == id)
            {
                conn.ForceCallNext();
            }
        }
    }

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
    public virtual Vector2 GetPosition(int id) { return Vector2.zero; }
    public virtual float GetFloat(int id) { return 0f; }

    public virtual void GameSetup(BehaviourSet bSet, BossData bData, GameObject bossReference)
    {
        behaviourSet = bSet;
        bossData = bData;
        boss = bossReference;
    }

    public bool IsType<T>() where T : BaseAction
    {
        return GetType().Equals(typeof(T));
    }
}
