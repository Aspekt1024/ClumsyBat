using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : ScriptableObject
{
    public List<InterfaceType> inputs = new List<InterfaceType>();
    public List<InterfaceType> outputs = new List<InterfaceType>();

    protected BossDataContainer owner;
    protected BossBehaviour bossBehaviour;
    protected GameObject boss;

    [System.Serializable]
    public struct InterfaceType
    {
        public int identifier;
        public BaseAction connectedAction;
        public int connectedInterfaceIndex;
    }

    public void Activate()
    {
        owner.CurrentAction = this;
        ActivateBehaviour();
    }

    public abstract void ActivateBehaviour();

    public virtual void Tick(float deltaTime) { }

    public void CallNext(int id = 0)
    {
        foreach (var output in outputs)
        {
            if (output.identifier == id && output.connectedAction != null)
            {
                output.connectedAction.Activate();
            }
        }
    }

    public BaseAction GetNextAction(int id = 0)
    {
        BaseAction nextAction = null;
        foreach (var output in outputs)
        {
            if (output.identifier == id && output.connectedAction != null)
            {
                nextAction = output.connectedAction;
                break;
            }
        }
        return nextAction;
    }

    public virtual GameObject GetObject(int id) { return null; }

    public virtual void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        owner = owningContainer;
        bossBehaviour = behaviour;
        boss = bossReference;
    }

    public bool IsType<T>() where T : BaseAction
    {
        return GetType().Equals(typeof(T));
    }
}
