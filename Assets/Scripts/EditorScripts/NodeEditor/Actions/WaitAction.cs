using UnityEngine;

public class WaitAction : BaseAction {
    
    public enum Ifaces
    {
        Input, Output
    }

    public float WaitTime = 1f;

    private bool bWaitActive;
    private float timeWaited;

    public override void ActivateBehaviour()
    {
        bWaitActive = true;
        timeWaited = 0f;
    }

    public override void Tick(float deltaTime)
    {
        if (!bWaitActive) return;
        timeWaited += deltaTime;
        if (timeWaited > WaitTime)
        {
            IsActive = false;
            bWaitActive = false;
            CallNext((int)Ifaces.Output);
        }
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        bWaitActive = false;
    }

}
