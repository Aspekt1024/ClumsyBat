﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class BossStateEditor : BaseEditor {
    
    [MenuItem("Window/Boss State Editor")]
    private static void ShowEditor()
    {
        BossStateEditor editor = GetWindow<BossStateEditor>();
        editor.SetEditorTheme();
        editor.StateMachine = null;
    }

    public override void LoadEditor(StateMachine obj)
    {
        StateMachine = obj;
        base.LoadEditor(obj);
    }

    protected override void SetEditorTheme()
    {
        if (StateMachine != null)
        {
            EditorLabel = string.Format("{0} - {1}", StateMachine.BossName, ((BossState)StateMachine).StateName);
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1Available");
        titleContent.text = "Boss State";
        colourTheme = ColourThemes.Blue;
    }

    // TODO needeD?
    private string GetNodeDataFolder()
    {
        string subFolder = GetSubfolderIfState(StateMachine);
        return "";//EditorHelpers.GetDataPath(BaseContainer.RootContainer) + (subFolder.Length > 0 ? "/" + subFolder : "");
    }

    private string GetSubfolderIfState(StateMachine container)
    {
        string subFolder = "";
        if (StateMachine.IsType<BossState>())
        {
            subFolder = StateMachine.name + "Data";
        }
        return subFolder;
    }
}