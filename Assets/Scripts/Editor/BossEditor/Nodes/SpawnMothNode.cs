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

    public override void DrawWindow()
    {
        WindowTitle = "Spawn Moth";
        WindowRect.width = 120;
        WindowRect.height = 60;

        SetInterfacePositions();
        DrawInterfaces();
    }

    protected override void CreateAction()
    {
        Action = CreateInstance<SpawnMothAction>();
    }
}
