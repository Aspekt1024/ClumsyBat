using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

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
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            editor.LoadLevel();
        }
        if (GUILayout.Button("Save"))
        {
            editor.SaveLevel();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Test"))
        {
            GameData.Instance.Level = editor.LevelId;
            Toolbox.Instance.Debug = editor.DebugMode;
            SceneManager.LoadScene("Levels");
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        // TODO insert horizontal line
        ShowLevelStats();

    }

    private void ShowLevelStats()
    {
        int numSections = editor.objectHandler.GetNumSections();
        float distance = numSections * LevelEditorConstants.TileSizeX;
        float timeTaken = distance / 6f;    // TODO set level speed somewhere else...... where?

        EditorGUILayout.LabelField("Num sections: " + numSections);
        EditorGUILayout.LabelField("Distance: " + distance);
        EditorGUILayout.LabelField("Time to complete: " + timeTaken + " sec");
    }

}
