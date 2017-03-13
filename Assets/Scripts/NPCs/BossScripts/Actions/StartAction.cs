using UnityEngine;

public class StartAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        CallNext();
        if (bossBehaviour != null)
            bossBehaviour.BossProps.LastStartingAction = this;
    }

}
