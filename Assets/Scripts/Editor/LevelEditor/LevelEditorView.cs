using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorView : Editor {

    LevelEditor editor;
    LevelEditorObjectHandler objectHandler;

    public override void OnInspectorGUI()
    {
        editor = (LevelEditor)target;
        if (objectHandler == null) objectHandler = new LevelEditorObjectHandler();

        editor.EditMode = EditorGUILayout.Toggle("Edit Mode", editor.EditMode);
        editor.DebugMode = EditorGUILayout.Toggle("Debug Mode", editor.DebugMode);
        editor.LevelId = (LevelProgressionHandler.Levels)EditorGUILayout.EnumPopup("Level", editor.LevelId);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            LoadLevel();
        }
        if (GUILayout.Button("Save"))
        {
            SaveLevel();
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
        int numSections = objectHandler.GetNumSections();
        float distance = numSections * LevelEditorConstants.TileSizeX;
        float timeTaken = distance / 6f;    // TODO set level speed somewhere else...... where?

        EditorGUILayout.LabelField("Num sections: " + numSections);
        EditorGUILayout.LabelField("Distance: " + distance);
        EditorGUILayout.LabelField("Time to complete: " + timeTaken + " sec");
    }
    
    public void LoadLevel()
    {
        SaveLevelHandler saveHandler = new SaveLevelHandler();
        saveHandler.Load(objectHandler, editor.LevelId);
    }

    public void SaveLevel()
    {
        SaveLevelHandler saveHandler = new SaveLevelHandler();
        saveHandler.Save(objectHandler, editor.LevelId);
    }
}
