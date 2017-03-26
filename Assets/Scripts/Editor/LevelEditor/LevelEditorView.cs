using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorView : Editor {

    LevelEditor editor;

    public override void OnInspectorGUI()
    {
        editor = (LevelEditor)target;

        editor.EditMode = EditorGUILayout.Toggle("Edit Mode", editor.EditMode);
        editor.DebugMode = EditorGUILayout.Toggle("Debug Mode", editor.DebugMode);
        editor.LevelId = (LevelProgressionHandler.Levels)EditorGUILayout.EnumPopup("Level", editor.LevelId);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            LoadLevel((int)editor.LevelId);
        }
        if (GUILayout.Button("Save"))
        {
            SaveLevel((int)editor.LevelId);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SaveLevel(int levelId)
    {
        editor.SaveLevel();
    }

    private void LoadLevel(int levelId)
    {
        editor.LoadLevel();
    }
}
