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
        if (State == null) return;
        bossData.BossStateMachine.ActivateNewState(State);
        State.BeginState();
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);

        string dataFolder = "NPCs/Bosses/BossBehaviours/Data";
        string bossFolder = behaviourSet.BossName.Replace(" ", "");
        string stateFolder = StateName.Replace(" ", "");
        string assetName = stateFolder;
        string resourcePath = string.Format("{0}/{1}/{2}/{3}", dataFolder, bossFolder, stateFolder, assetName);
        
        State = Resources.Load<State>(resourcePath);
        if (State == null) return;
        
        State.SetupActions(bossData, bossReference);
        State.bEnabled = false;
    }

    public override void Tick(float deltaTime)
    {
        if (State == null || !State.bEnabled) return;
        State.Tick(deltaTime);
    }
}
