using UnityEngine;

public class LoopAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        Debug.Log("loop");
        ParentStateMachine.RequestLoopToStart();
    }
}
