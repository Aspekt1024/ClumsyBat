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
        editor.BehaviourSet = null;
    }

    public override void LoadEditor(BehaviourSet obj)
    {
        BehaviourSet = obj;
        base.LoadEditor(obj);
    }

    protected override void SetEditorTheme()
    {
        if (BehaviourSet != null)
        {
            EditorLabel = string.Format("{0} - {1}", BehaviourSet.BossName, ((State)BehaviourSet).StateName);
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1Available");
        titleContent.text = "Boss State";
        colourTheme = ColourThemes.Blue;
    }
}
