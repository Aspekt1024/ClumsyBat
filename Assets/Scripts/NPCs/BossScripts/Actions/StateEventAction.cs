using UnityEngine;

public class StateEventAction : BaseAction {
    
    public int StateEventID;

    public enum Ifaces
    {
        Input
    }

    public override void ActivateBehaviour()
    {
        ((BossState)ParentStateMachine).ActivateEvent(StateEventID);
    }

}
