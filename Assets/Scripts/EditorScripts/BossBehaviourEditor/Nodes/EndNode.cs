using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EndNode : BaseNode {
    

    public override void SetupNode()
    {
        WindowTitle = "End";
        WindowRect.width = 100;
        WindowRect.height = 80;
        AddInput(WindowRect.height / 2);
    }

    public override void DrawWindow()
    {
        DrawInterfaces();
    }

    public override void Tick(float DeltaTime)
    {
    }
}