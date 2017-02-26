using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class StartNode : BaseNode {

    private void OnEnable()
    {
        AddOutput();
    }

    public override void SetWindowRect(Vector2 mousePos)
    {
        width = 100;
        height = 80;
        WindowTitle = "Start";

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
