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

    public override void GameSetup(StateMachine owningContainer, BossData behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        jumpAbility = bossBehaviour.GetAbility<JumpPound>();
    }

    public override void ActivateBehaviour()
    {
        boss.GetComponent<Boss>().Jump();
        jumpAbility.Jump(this, JumpForce);
        CallNext((int)Ifaces.Jumped);
    }

    public void Landed()
    {
        if (bossBehaviour.BossProps.ShakeScreenOnLanding)
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
}
