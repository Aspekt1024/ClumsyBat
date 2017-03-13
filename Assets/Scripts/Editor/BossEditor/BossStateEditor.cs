using UnityEngine;
using UnityEditor;
using System;

public class BossStateEditor : BaseEditor {
    
    [MenuItem("Window/Boss State Editor")]
    private static void ShowEditor()
    {
        BossStateEditor editor = GetWindow<BossStateEditor>();
        editor.SetEditorTheme();
        editor.BaseContainer = null;
    }

    public override void LoadEditor(BossDataContainer obj)
    {
        BaseContainer = obj;
        base.LoadEditor(obj);
    }

    protected override void SetEditorTheme()
    {
        if (BaseContainer != null)
        {
            EditorLabel = string.Format("{0} - {1}", BaseContainer.BossName, ((BossState)BaseContainer).StateName);
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1Available");
        titleContent.text = "Boss State";
        colourTheme = ColourThemes.Blue;
    }

    protected override void LoadNodeData()
    {
        string nodeDataName = BaseContainer.name.Replace(" ", "");
        string nodeDataFolder = EditorHelpers.GetDataPath(BaseContainer.RootContainer);
        string nodeDataPath = string.Format("{0}/States/{1}.asset", nodeDataFolder, nodeDataName); // TODO this must match what's in StateNode... merge into one static function e.g. GetStateNodePath(...)

        NodeData = AssetDatabase.LoadAssetAtPath<BossEditorNodeData>(nodeDataPath);

        if (NodeData == null)
        {
            EditorHelpers.CreateFolderIfNotExist(nodeDataFolder, "States");
            CreateNewNodeData(nodeDataPath);
        }
    }

    private string GetNodeDataFolder()
    {
        string subFolder = GetSubfolderIfState(BaseContainer);
        return EditorHelpers.GetDataPath(BaseContainer.RootContainer) + (subFolder.Length > 0 ? "/" + subFolder : "");
    }

    private string GetSubfolderIfState(BossDataContainer container)
    {
        string subFolder = "";
        if (BaseContainer.IsType<BossState>())
        {
            subFolder = BaseContainer.name + "Data";
        }
        return subFolder;
    }
}
