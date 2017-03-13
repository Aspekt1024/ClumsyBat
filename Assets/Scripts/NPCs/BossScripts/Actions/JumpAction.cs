using UnityEngine;

[System.Serializable]
public class JumpAction : BaseAction {
    
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
        jumpAbility = (JumpPound)bossBehaviour.GetAbility<JumpPound>();
    }

    public override void ActivateBehaviour()
    {
        jumpAbility.Jump(this);
        CallNext((int)Outputs.Jumped);
    }

    public void Landed()
    {
        if (bossBehaviour.BossProps.ShakeScreenOnLanding)
        {
            CameraEventListener.CameraShake(0.4f);
        }
        CallNext((int)Outputs.Landed);
    }

    public void TopOfJump()
    {
        CallNext((int)Outputs.Top);
    }
}
