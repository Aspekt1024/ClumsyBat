using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpeechNode : BaseNode {
    
    public override void SetWindowRect(Vector2 mousePos)
    {
        width = 200;
        height = 150;
        WindowTitle = "Speech";

        base.SetWindowRect(mousePos);
    }
}
