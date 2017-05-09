using UnityEngine;

[System.Serializable]
public class JumpAction : BaseAction {

    public float JumpForce;

    private JumpPound jumpAbility;
    
    public enum Outputs
    {
        Jumped,
        Top,
        Landed
    }

    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        jumpAbility = bossBehaviour.GetAbility<JumpPound>();
    }

    public override void ActivateBehaviour()
    {
        boss.GetComponent<Boss>().Jump();
        jumpAbility.Jump(this, JumpForce);
        CallNext((int)Outputs.Jumped);
    }

    public void Landed()
    {
        if (bossBehaviour.BossProps.ShakeScreenOnLanding)
        {
            CameraEventListener.CameraShake(0.4f);
        }
        boss.GetComponent<Boss>().EndJump();
        CallNext((int)Outputs.Landed);
    }

    public void TopOfJump()
    {
        CallNext((int)Outputs.Top);
    }
}
