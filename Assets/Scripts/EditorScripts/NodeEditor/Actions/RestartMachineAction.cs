using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartMachineAction : BaseAction {

    protected override void ActivateBehaviour()
    {
        IsActive = false;
        behaviourSet.ParentMachine.StartMachine();
    }
}
