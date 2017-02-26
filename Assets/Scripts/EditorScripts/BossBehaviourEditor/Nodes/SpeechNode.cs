using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpeechNode : BaseNode {

    private GameObject _boss;

    private float elapsedTime;
    private const float timeToAnnounce = 5f;
    private bool _bAnnounced;

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
