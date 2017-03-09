using UnityEngine;

public class LoopAction : BaseAction {
    
    public override void Activate()
    {
        bossBehaviour.BossProps.LastStartingAction.Activate();
    }
}
