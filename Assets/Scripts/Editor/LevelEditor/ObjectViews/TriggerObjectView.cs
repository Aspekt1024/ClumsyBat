using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(TriggerClass))]
public class TriggerEditorView : Editor {

    TriggerClass trigger;

    public override void OnInspectorGUI()
    {
        trigger = (TriggerClass)target;

        trigger.EventType = (TriggerHandler.EventType)EditorGUILayout.EnumPopup("Event Type", trigger.EventType);

        if (trigger.EventType == TriggerHandler.EventType.OneTimeEvent || trigger.EventType == TriggerHandler.EventType.Dialogue)
        {
            trigger.EventId = (TooltipHandler.DialogueId)EditorGUILayout.EnumPopup("ID", trigger.EventId);
        }
        else
        {
            EditorStyles.textField.wordWrap = true;
            trigger.TooltipText = EditorGUILayout.TextArea(trigger.TooltipText);
        }
        trigger.TooltipDuration = EditorGUILayout.FloatField("Duration (s)", trigger.TooltipDuration);
        trigger.PausesGame = EditorGUILayout.Toggle("Pauses Game?", trigger.PausesGame);
        
        CopyTriggerClassProperties();
    }

    private void CopyTriggerClassProperties()
    {
        ((TriggerClass)target).EventType = trigger.EventType;
        ((TriggerClass)target).EventId = trigger.EventId;
        ((TriggerClass)target).TooltipText = trigger.TooltipText;
        ((TriggerClass)target).TooltipDuration = trigger.TooltipDuration;
        ((TriggerClass)target).PausesGame = trigger.PausesGame;
    }
}
