using UnityEngine;

public class StartAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        Debug.Log("starting");
        CallNext(0);
    }

    public override void GameSetup(StateMachine parentMachine, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(parentMachine, bossData, bossReference);
        ParentStateMachine.StartingAction = this;
    }

}
