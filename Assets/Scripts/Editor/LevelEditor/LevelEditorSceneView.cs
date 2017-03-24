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
        GameObject scriptsObject = GameObject.FindGameObjectWithTag("Scripts");
        if (scriptsObject == null) return;
        LevelEditor editor = scriptsObject.GetComponent<LevelEditor>();
        if (editor != null && editor.EditMode)
        {
            editor.ProcessEvent();
        }
    }
}

