using UnityEngine;

public class StartAction : BaseAction {

    public int TestVariable = 199;

    public override void Activate()
    {
        CallNext();
        if (bossBehaviour != null)
            bossBehaviour.BossProps.LastStartingAction = this;
    }

}
