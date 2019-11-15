using System;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class LevelEditorSceneView
{
    static LevelEditorSceneView()
    {
        SceneView.duringSceneGui += OnSceneGUI;
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

            if (editor.EditMode)
            {
                var editorActions = new LevelEditorActions();
                editorActions.ProcessEvent(mousePosition, editor);
            }
        }
    }
}

