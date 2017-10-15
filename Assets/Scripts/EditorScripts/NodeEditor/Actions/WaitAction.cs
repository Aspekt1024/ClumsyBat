using UnityEngine;

public class WaitAction : BaseAction {
    
    public enum Ifaces
    {
        Input, Output
    }

    public float WaitTime = 1f;

    private bool bWaitActive;
    private float timeWaited;

    protected override void ActivateBehaviour()
    {
        bWaitActive = true;
        timeWaited = 0f;
    }

    public override void Tick(float deltaTime)
    {
        if (!bWaitActive || !IsActive) return;
        timeWaited += deltaTime;
        if (timeWaited > WaitTime)
        {
            bWaitActive = false;
            IsActive = false;
            CallNext((int)Ifaces.Output);
        }
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        bWaitActive = false;
    }

    public override void Stop()
    {
        IsStopped = true;
        if (!IsActive) return;
        IsActive = false;
        bWaitActive = false;
    }

}
