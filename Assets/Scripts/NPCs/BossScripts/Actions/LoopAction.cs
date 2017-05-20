using UnityEngine;

public class LoopAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        Debug.Log(Time.time + " : loop");
        owner.RequestLoopToStart();
    }
}
