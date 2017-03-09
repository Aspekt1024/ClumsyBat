using UnityEngine;

public class MachineState : BaseAction {

    public BossState State; // TODO remove?
    public StartAction StartingAction;
    
    public override void Activate()
    {
        StartingAction.Activate();
    }

}
