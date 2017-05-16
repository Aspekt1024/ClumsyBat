using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnMothNode : BaseNode {
    
    protected override void AddInterfaces()
    {
        AddInput();
    }

    private void SetInterfacePositions()
    {
        SetInput(30);
    }

    public override void Draw()
    {
        WindowTitle = "Spawn Moth";
        Transform.Width = 120;
        Transform.Height = 60;

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<SpawnMothAction>();
    }
}
