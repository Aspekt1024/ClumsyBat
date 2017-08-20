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
        editor.ScoreToBeat = EditorGUILayout.IntField("Score to Beat:", editor.ScoreToBeat);
        ShowLevelStats();

    }

    private void ShowLevelStats()
    {
        int numSections = objectHandler.GetNumSections();
        int numMoths = objectHandler.GetNumMoths();
        float distance = numSections * LevelEditorConstants.TileSizeX - 12.55f;
        float timeTaken = distance / 5.5f;    // TODO set level speed somewhere else...... where?

        EditorGUILayout.LabelField("Num sections: " + numSections);
        EditorGUILayout.LabelField("Distance: " + distance);
        EditorGUILayout.LabelField("Num Moths: " + numMoths);
        EditorGUILayout.LabelField("Time to complete: " + timeTaken + " sec");
        EditorGUILayout.LabelField("Suggested score: " + ScoreCalculator.SuggestScore(distance, numMoths));
    }
    
    private void LoadLevel()
    {
        SaveLevelHandler saveHandler = new SaveLevelHandler();
        saveHandler.Load(objectHandler, editor.LevelId);
    }

    private void SaveLevel()
    {
        SaveLevelHandler saveHandler = new SaveLevelHandler(); 
        saveHandler.Save(objectHandler, editor.LevelId, editor.ScoreToBeat);
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
