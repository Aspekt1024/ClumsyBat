using UnityEngine;

public class LoopAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        Active = false;
        ParentStateMachine.RequestLoopToStart();
    }
}
