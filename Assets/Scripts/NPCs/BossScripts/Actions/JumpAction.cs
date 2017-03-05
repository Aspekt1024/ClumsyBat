using UnityEngine;

public class JumpAction : BaseAction {

    private JumpPound jumpAbility;
    
    public enum Outputs
    {
        Jumped,
        Top,
        Landed
    }

    public override void GameSetup(BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(behaviour, bossReference);
        jumpAbility = (JumpPound)bossBehaviour.GetAbility<JumpPound>();
    }

    public override void Activate()
    {
        jumpAbility.Jump(this);
        CallNext((int)Outputs.Jumped);
    }

    public void Landed()
    {
        CallNext((int)Outputs.Landed);
    }

    public void TopOfJump()
    {
        CallNext((int)Outputs.Top);
    }
}
