public class BossEvents {

    public delegate void BossEventHandler();
    public static BossEventHandler OnJumpLanded;
    public static BossEventHandler OnBossFightStart;
    public static BossEventHandler OnBossDeath;
    
    public delegate void BossWaitEvent(float waitTime, BaseAction caller);
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

    public static void Wait(float waitTime, BaseAction caller)
    {
        if (OnWait != null)
            OnWait(waitTime, caller);
    }

    public static void BossDeath()
    {
        if (OnBossDeath != null)
            OnBossDeath();
    }
}

