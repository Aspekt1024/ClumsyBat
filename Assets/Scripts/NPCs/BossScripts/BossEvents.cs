public class BossEvents {

    public delegate void BossEventHandler();
    public static BossEventHandler OnJumpLanded;

    public static void JumpLanded()
    {
        if (OnJumpLanded != null)
            OnJumpLanded();
    }
}

