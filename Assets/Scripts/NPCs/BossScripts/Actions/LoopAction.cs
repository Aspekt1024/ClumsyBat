using UnityEngine;

public class LoopAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        owner.RequestLoopToStart();
    }
}
