using UnityEngine;

public class TimerAction : BaseAction {
    
    public enum Ifaces
    {
        Start, Reset, Value
    }

    public float StartingValue;

    private float timer;
    private bool isRunning;

    protected override void ActivateBehaviour()
    {
        IsActive = false;
        if (GetInterface((int)Ifaces.Start).WasCalled())
        {
            GetInterface((int)Ifaces.Start).UseCall();
            if (!isRunning)
            {
                timer = StartingValue;
                isRunning = true;
            }
        }
        else if (GetInterface((int)Ifaces.Reset).WasCalled())
        {
            GetInterface((int)Ifaces.Reset).UseCall();
            timer = 0;
        }
    }

    public override void Tick(float deltaTime)
    {
        if (!IsActive) return;
        if (!Toolbox.Instance.GamePaused && isRunning)
        {
            timer += deltaTime;
        }
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        timer = 0f;
        isRunning = false;
    }

    public override void Stop()
    {
        IsStopped = true;
        IsActive = false;
        isRunning = false;
    }

    public override float GetFloat(int id)
    {
        if (id == (int)Ifaces.Value)
            return timer;
        else
            return 0;
    }
}
