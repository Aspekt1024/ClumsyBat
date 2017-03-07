
using System;

public class ParabolicProjectileNode : BaseNode {

    protected override void AddInterfaces()
    {
        AddInput();
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);
        SetOutput(WindowRect.height / 2);
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
