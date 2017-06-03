using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class StateAction : BaseAction {
    
    public string StateName;

    [XmlIgnore] public State State;
    
    public override void ActivateBehaviour()
    {
        if (State == null)
        {
            Debug.LogError("Could not find State in State Action " + StateName);
            return;
        }
        bossData.BossStateMachine.ActivateNewState(State);
        State.BeginState();
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);

        string dataFolder = BossActionLoadHandler.ResourcesDataFolder;
        string bossFolder = behaviourSet.name;
        string stateFolder = StateName;
        string assetName = stateFolder;
        string resourcePath = string.Format("{0}/{1}/{2}/{3}", dataFolder, bossFolder, stateFolder, assetName);
        
        State = Resources.Load<State>(resourcePath);
        if (State == null) return;

        State.SetupActions(bossData, bossReference);
        State.IsEnabled = false;
    }

    public override void Tick(float deltaTime)
    {
        if (State == null || !State.IsEnabled) return;
        State.Tick(deltaTime);
    }

    public override void Stop()
    {
        if (!IsActive) return;

        IsActive = false;
        foreach (var action in State.Actions)
        {
            action.Stop();
        }
    }
}
