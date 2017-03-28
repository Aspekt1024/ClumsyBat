using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorView : Editor {

    private LevelEditor editor;
    private LevelEditorObjectHandler objectHandler;

    private bool testClicked = false;

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
        if (GUILayout.Button("Load " + editor.LevelId + ".xml")) LoadLevel();
        if (GUILayout.Button("Save " + editor.LevelId + ".xml")) SaveLevel();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (testClicked)
        {
            TestLevel();
        }
        else
        {
            if (GUILayout.Button("Test"))
                testClicked = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        ShowLevelStats();

    }

    private void ShowLevelStats()
    {
        int numSections = objectHandler.GetNumSections();
        float distance = numSections * LevelEditorConstants.TileSizeX;
        float timeTaken = distance / 5f;    // TODO set level speed somewhere else...... where?

        EditorGUILayout.LabelField("Num sections: " + numSections);
        EditorGUILayout.LabelField("Distance: " + distance);
        EditorGUILayout.LabelField("Time to complete: " + timeTaken + " sec");
    }
    
    private void LoadLevel()
    {
        SaveLevelHandler saveHandler = new SaveLevelHandler();
        saveHandler.Load(objectHandler, editor.LevelId);
    }

    private void SaveLevel()
    {
        SaveLevelHandler saveHandler = new SaveLevelHandler(); 
        saveHandler.Save(objectHandler, editor.LevelId);
        AssetDatabase.Refresh();
    }

    private void TestLevel()
    {
        if (GUILayout.Button("Cancel"))
            testClicked = false;

        EditorGUILayout.Space();

        if (GUILayout.Button("Overwrite " + editor.LevelId + ".xml and test!"))
        {
            SaveLevel();
            EditorApplication.isPlaying = true;
        }
    }
}
