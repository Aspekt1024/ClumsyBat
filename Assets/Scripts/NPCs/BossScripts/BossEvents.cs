public class BossEvents {

    public delegate void BossEventHandler();
    public static BossEventHandler OnJumpLanded;
    public static BossEventHandler OnBossFightStart;

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

}

