using UnityEngine;

[System.Serializable]
public class JumpAction : BaseAction {

    public float JumpForce;

    private JumpPound jumpAbility;
    
    public enum Ifaces
    {
        Input,
        Jumped,
        Top,
        Landed
    }

    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        jumpAbility = base.bossData.GetAbility<JumpPound>();
    }

    protected override void ActivateBehaviour()
    {
        boss.GetComponent<Boss>().Jump();
        jumpAbility.Jump(this, JumpForce);
        CallNext((int)Ifaces.Jumped);
    }

    public void Landed()
    {
        IsActive = false;
        if (bossData.BossStateMachine.ShakeScreenOnLanding) // TODO this is a property of jump, not the boss
        {
            CameraEventListener.CameraShake(0.4f);
        }
        boss.GetComponent<Boss>().EndJump();
        CallNext((int)Ifaces.Landed);
    }

    public void TopOfJump()
    {
        CallNext((int)Ifaces.Top);
    }

    public override void Stop()
    {
        IsStopped = true;
        if (!IsActive) return;

        boss.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        IsActive = false;
        jumpAbility.StopAllCoroutines();
    }
}
