using UnityEngine;

public class LoopAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        bossBehaviour.BossProps.LastStartingAction.Activate();
    }
}
