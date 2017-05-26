﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class BossEditor : BaseEditor
{
    [MenuItem("Window/Boss Editor")]
    private static void ShowEditor()
    {
        BossEditor editor = GetWindow<BossEditor>();
        editor.SetEditorTheme();
        editor.BehaviourSet = null;
    }

    protected override void SetEditorTheme()
    {
        if (BehaviourSet != null)
        {
            EditorLabel = "State machine : " + BehaviourSet.Name;
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1AvailableClicked");
        titleContent.text = "Boss Editor";
        colourTheme = ColourThemes.Black;
    }

    public void EditState()
    {
        State state = ((StateNode)_currentNode).State;
        LoadEditor(state);
    }
}
