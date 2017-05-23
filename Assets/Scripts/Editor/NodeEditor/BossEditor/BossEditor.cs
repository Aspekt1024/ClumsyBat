using UnityEngine;
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
        editor.StateMachine = null;
    }

    protected override void SetEditorTheme()
    {
        if (StateMachine != null)
        {
            EditorLabel = "State machine : " + StateMachine.BossName;
        }

        titleContent.image = (Texture)Resources.Load("LevelButtons/Boss1AvailableClicked");
        titleContent.text = "Boss Editor";
        colourTheme = ColourThemes.Black;
    }

    public void EditState()
    {
        BossState state = ((StateNode)_currentNode).State;
        LoadEditor(state);
    }
}
