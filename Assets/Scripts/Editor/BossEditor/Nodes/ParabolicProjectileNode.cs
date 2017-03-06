
public class ParabolicProjectileNode : BaseNode {
    
    public override void SetupNode(BossDataContainer dataContainer)
    {
        DataContainer = dataContainer;
        SaveThisNodeAsset();
        
        Action = CreateInstance<ProjectileAction>();
        SaveActionAsset();

        AddInput();
        AddOutput();

        UpdateActionInterfaces();
    }

    private void SetInterfacePositions()
    {
        CreateInput(WindowRect.height / 2);
        CreateOutput(WindowRect.height / 2);
    }

    public override void DrawWindow()
    {
        WindowTitle = "Parabolic Projectile";
        WindowRect.width = 150;
        WindowRect.height = 60;


        SetInterfacePositions();
        DrawInterfaces();
    }
}
