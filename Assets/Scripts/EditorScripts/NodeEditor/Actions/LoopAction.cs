using UnityEngine;

public class LoopAction : BaseAction {

    protected override void ActivateBehaviour()
    {
        IsActive = false;
        behaviourSet.LoopToStart();
    }
}
