using UnityEngine;
using UnityEditor;
using System;

using Outputs = ProjectileAction.Outputs;

// TODO rename to ProjectileNode- this will do both parabolic and straight
public class ParabolicProjectileNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInput();

        AddOutput((int)Outputs.Launched);
        AddOutput((int)Outputs.Landed);
        AddOutput((int)Outputs.HitPlayer);
        AddOutput((int)Outputs.Projectile, InterfaceTypes.Object);
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);

        SetOutput(40, (int)Outputs.Launched, "Launched");
        SetOutput(55, (int)Outputs.HitPlayer, "Player Hit");
        SetOutput(70, (int)Outputs.Landed, "Landed");
        SetOutput(95, (int)Outputs.Projectile, "Projectile");
    }

    public override void DrawWindow()
    {
        WindowTitle = "Parabolic Projectile";
        WindowRect.width = 150;
        WindowRect.height = 120;
        
        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<ProjectileAction>();
    }
}
