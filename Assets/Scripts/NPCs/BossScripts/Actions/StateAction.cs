using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class StateAction : BaseAction {
    
    public enum BossDamageObjects
    {
        Hypersonic, Stalactite, Player
    }

    public string StateName;
    [XmlIgnore] public BossState State;

    //public List<BossDamageObjects> damageObjects = new List<BossDamageObjects>(); // TODO could use this here instead
    
    public override void ActivateBehaviour()
    {
        if (State == null) return;
        bossData.BossStateMachine.ActivateNewState(State);
        State.BeginState();
    }

    public override void GameSetup(StateMachine parentMachine, BossData bossdata, GameObject bossReference)
    {
        base.GameSetup(parentMachine, bossdata, bossReference);

        string dataFolder = "NPCs/Bosses/BossBehaviours/Data";
        string subFolder = parentMachine.BossName.Replace(" ", "");
        string assetName = StateName.Replace(" ", "") + ".asset";
        string resourcePath = string.Format("{0}/{1}/{2}", dataFolder, subFolder, assetName);

        State = Resources.Load<BossState>(resourcePath);
        if (State == null) return;

        State.SetupActions(bossdata, bossReference);
        State.bEnabled = false;
    }

    public override void Tick(float deltaTime)
    {
        if (State == null || !State.bEnabled) return;
        State.Tick(deltaTime);
    }

}
