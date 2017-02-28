using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpeechNode : BaseNode {

    private GameObject _boss;

    private float elapsedTime;
    private const float timeToAnnounce = 5f;
    private bool _bAnnounced;
    
    public override void SetupNode()
    {
        WindowRect.width = 200;
        WindowRect.height = 150;
        WindowTitle = "Speech";
        AddInput(WindowRect.height / 2);
        AddOutput(WindowRect.height / 2);
    }

    public override void DrawWindow()
    {
        _boss = (GameObject)EditorGUILayout.ObjectField(_boss, typeof(GameObject), true);
        DrawInterfaces();
    }

    public override void Tick(float deltaTime)
    {
        elapsedTime += deltaTime;
        if (elapsedTime > timeToAnnounce && !_bAnnounced && _boss != null)
        {
            _bAnnounced = true;
            Debug.Log("I am " + _boss.name);
        }
    }
}
