using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EndNode : BaseNode {
    

    public override void SetupNode()
    {
        AddInput();
    }

    public override void SetWindowRect(Vector2 mousePos)
    {
        width = 100;
        height = 80;
        WindowTitle = "End";

        base.SetWindowRect(mousePos);
    }

    public override void DrawWindow()
    {
        DrawInterfaces();
    }

    public override void Tick(float DeltaTime)
    {
    }
}