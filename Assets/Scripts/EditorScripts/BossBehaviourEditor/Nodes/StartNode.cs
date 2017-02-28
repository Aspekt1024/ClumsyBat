using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class StartNode : BaseNode {
    
    public override void SetupNode()
    {
        WindowRect.width = 100;
        WindowRect.height = 80;
        WindowTitle = "Start";
        AddOutput(WindowRect.height / 2);
    }
    
    public override void DrawWindow()
    {
        DrawInterfaces();
    }

    public override void Tick(float DeltaTime)
    {
    }
}
