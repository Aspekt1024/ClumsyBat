using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : ScriptableObject {
    
    public List<InterfaceType> inputs = new List<InterfaceType>();
    public List<InterfaceType> outputs = new List<InterfaceType>();

    protected BossBehaviour bossBehaviour;
    protected GameObject boss;

    [System.Serializable]
    public struct InterfaceType
    {
        public int identifier;
        public BaseAction connectedAction;
        public int connectedInterfaceIndex;
    }

    public abstract void Activate();

    public void CallNext(int id = 0)
    {
        foreach (var output in outputs)
        {
            if (output.identifier == id && output.connectedAction != null)
                output.connectedAction.Activate();
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

    public virtual void GameSetup(BossBehaviour behaviour, GameObject bossReference)
    {
        bossBehaviour = behaviour;
        boss = bossReference;
    }
}
