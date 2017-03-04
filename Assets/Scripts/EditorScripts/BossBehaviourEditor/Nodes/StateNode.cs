using UnityEditor;
using UnityEngine;

public class StateNode : BaseNode {

    public BossState State;

    public override void SetupNode()
    {
        AddInput();
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(25f);
        SetOutput(25f);
    }

    public override void DrawWindow()
    {
        WindowRect.width = 200;
        WindowRect.height = 100;
        WindowTitle = State.StateName;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 80;
        State.StateName = EditorGUILayout.TextField("State name:", State.StateName);

        SetInterfacePositions();
        DrawInterfaces();
    }

    public override void Activate()
    {
        Debug.Log("state node activated");
    }

    public void CreateNewState(string dataFolder, string bossName)
    {
        State = CreateInstance<BossState>();
        State.BossName = bossName;

        EditorHelpers.CreateFolderIfNotExist(dataFolder.Substring(0, dataFolder.Length - 1), "States");
        EditorHelpers.SaveObjectToFolder(State, dataFolder + "States/", "State");
        EditorUtility.SetDirty(State);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public override void DeleteNode()
    {
        base.DeleteNode();

        // TODO decide if we want to do this.
        // Remove if no decision by 04.04.17
        /*if (AssetDatabase.Contains(State))
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(State));

        State = null;*/
    }

}
