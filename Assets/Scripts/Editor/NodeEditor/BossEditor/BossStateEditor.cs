using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

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

    // TODO needeD?
    private string GetNodeDataFolder()
    {
        string subFolder = GetSubfolderIfState(BaseContainer);
        return "";//EditorHelpers.GetDataPath(BaseContainer.RootContainer) + (subFolder.Length > 0 ? "/" + subFolder : "");
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
