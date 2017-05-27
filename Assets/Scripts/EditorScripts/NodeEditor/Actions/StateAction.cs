using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class StateAction : BaseAction {
    
    public enum BossDamageObjects
    {
        Hypersonic, Stalactite, Player
    }

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

        Debug.Log("loading state at " + resourcePath);
        State = Resources.Load<State>(resourcePath);
        if (State == null) return;
        Debug.Log("loaded successfully");

        State.SetupActions(bossData, bossReference);
        State.IsEnabled = false;
    }

    public override void Tick(float deltaTime)
    {
        if (State == null || !State.IsEnabled) return;
        State.Tick(deltaTime);
    }
}
