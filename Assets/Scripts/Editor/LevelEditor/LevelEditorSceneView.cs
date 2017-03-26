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
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
            mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
            mousePosition.z = 0f;
            editor.ProcessEvent(mousePosition);
        }
    }
}

