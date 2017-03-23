using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public static class LevelEditorSceneView
{
    static LevelEditorSceneView()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        LevelEditor editor = GameObject.FindGameObjectWithTag("Scripts").GetComponent<LevelEditor>();
        if (editor != null && editor.IsInEditMode)
        {
            editor.ProcessEvent();
        }
    }
}

