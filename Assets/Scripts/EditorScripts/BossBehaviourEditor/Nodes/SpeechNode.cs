using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpeechNode : BaseNode {
    
    private void OnEnable()
    {
        AddInput();
        AddOutput();
    }

    public override void SetWindowRect(Vector2 mousePos)
    {
        width = 200;
        height = 150;
        WindowTitle = "Speech";

        base.SetWindowRect(mousePos);
    }

    public override void DrawWindow()
    {
        WindowTitle = EditorGUILayout.TextField("Title", WindowTitle);
        DrawOutput();
    }
}
