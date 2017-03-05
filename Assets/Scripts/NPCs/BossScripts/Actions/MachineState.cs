using UnityEngine;

public class MachineState : BaseAction {

    public BossState State;
    
    public override void Activate()
    {
        Debug.Log("state node activated");
    }

}
