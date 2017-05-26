using UnityEngine;

public class LoopAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        IsActive = false;
        behaviourSet.RequestLoopToStart();
    }
}
