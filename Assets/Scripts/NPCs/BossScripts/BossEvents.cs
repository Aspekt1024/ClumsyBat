public class BossEvents {

    public delegate void BossEventHandler();
    public static BossEventHandler OnJumpLanded;
    public static BossEventHandler OnBossFightStart;
    
    public delegate void BossWaitEvent(float waitTime, BaseNode caller);
    public static BossWaitEvent OnWait;

    public static void JumpLanded()
    {
        if (OnJumpLanded != null)
            OnJumpLanded();
    }

    public static void BossFightStart()
    {
        if (OnBossFightStart != null)
            OnBossFightStart();
    }

    public static void Wait(float waitTime, BaseNode caller)
    {
        if (OnWait != null)
            OnWait(waitTime, caller);
    }
}

