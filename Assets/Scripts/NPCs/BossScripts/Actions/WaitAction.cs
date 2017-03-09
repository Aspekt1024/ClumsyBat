using UnityEngine;

public class WaitAction : BaseAction {

    public float WaitTime = 1f;

    public override void Activate()
    {
        BossEvents.Wait(WaitTime, this);
    }
    public void WaitComplete()
    {
        CallNext();
    }
}
