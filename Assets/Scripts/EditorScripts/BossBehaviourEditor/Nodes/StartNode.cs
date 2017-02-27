using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class StartNode : BaseNode {
    
    public override void SetupNode()
    {
        AddOutput();
    }

    public override void SetWindowRect(Vector2 position)
    {
        width = 100;
        height = 80;
        WindowTitle = "Start";

        base.SetWindowRect(position);
    }

    public override void DrawWindow()
    {
        DrawInterfaces();
    }

    public override void Tick(float DeltaTime)
    {
    }
}
