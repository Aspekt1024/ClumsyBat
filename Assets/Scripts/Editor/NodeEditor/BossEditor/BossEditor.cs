using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class BossEditor : BaseEditor {
    
    public enum EditorType
    {
        StateMachine, State
    }
    public EditorType type;

    [MenuItem("Window/Boss Editor")]
    private static void ShowEditor()
    {
        BossEditor editor = GetWindow<BossEditor>();
        editor.SetEditorTheme();
        editor.StateMachine = null;
    }

    public override void LoadEditor(StateMachine creatorObj)
    {
        StateMachine = creatorObj;
        if (StateMachine.IsType<BossStateMachine>())
        {
            type = EditorType.StateMachine;
        }
        else if (StateMachine.IsType<BossState>())
        {
            type = EditorType.State;
        }

        base.LoadEditor(creatorObj);
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
        // TODO this only works if the editor is already active...
        BossState state = ((StateNode)_currentNode).State;
        BossStateEditor editor = GetWindow<BossStateEditor>(desiredDockNextTo: typeof(SceneView));
        editor.LoadEditor(state);
    }
}
