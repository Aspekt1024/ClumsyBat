using UnityEngine;

public class JumpNode : BaseNode {
    
    private JumpPound jumpAbility;

    private enum Outputs
    {
        Jumped,
        Top,
        Landed
    }

    public override void SetupNode()
    {
        AddInput();
        AddOutput((int)Outputs.Jumped);
        AddOutput((int)Outputs.Top);
        AddOutput((int)Outputs.Landed);
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);
        SetOutput(40, (int)Outputs.Jumped, "Jumped");
        SetOutput(60, (int)Outputs.Top, "Top");
        SetOutput(80, (int)Outputs.Landed, "Landed");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Jump";
        WindowRect.width = 150;
        WindowRect.height = 100;

        
        SetInterfacePositions();
        DrawInterfaces();
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
