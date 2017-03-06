using Outputs = JumpAction.Outputs;

public class JumpNode : BaseNode {
    
    public override void SetupNode(BossDataContainer dataContainer)
    {
        DataContainer = dataContainer;
        SaveThisNodeAsset();

        Action = CreateInstance<JumpAction>();
        SaveActionAsset();

        AddInput();

        AddOutput((int)Outputs.Jumped);
        AddOutput((int)Outputs.Top);
        AddOutput((int)Outputs.Landed);

        UpdateActionInterfaces();
    }

    private void SetInterfacePositions()
    {
        CreateInput(WindowRect.height / 2);
        CreateOutput(40, (int)Outputs.Jumped, "Jumped");
        CreateOutput(60, (int)Outputs.Top, "Top");
        CreateOutput(80, (int)Outputs.Landed, "Landed");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Jump";
        WindowRect.width = 150;
        WindowRect.height = 100;

        
        SetInterfacePositions();
        DrawInterfaces();
    }
    
}
