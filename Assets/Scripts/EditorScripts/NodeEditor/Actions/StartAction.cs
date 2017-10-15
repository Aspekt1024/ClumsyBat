using UnityEngine;

public class StartAction : BaseAction {

    protected override void ActivateBehaviour()
    {
        Debug.Log("startcalled");
        IsActive = false;
        CallNext(0);
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        base.behaviourSet.StartingAction = this;
    }

}
