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
        TriggerEvent trigEvent = trigger.TriggerEvent;

        trigEvent.EventType = (TriggerHandler.EventType)EditorGUILayout.EnumPopup("Event Type", trigEvent.EventType);

        EditorStyles.textField.wordWrap = true;
        for (int i = 0; i < trigger.TriggerEvent.Dialogue.Count; i++)
        {
            trigger.TriggerEvent.Dialogue[i] = EditorGUILayout.TextField(trigger.TriggerEvent.Dialogue[i]);
        }

        if (GUILayout.Button("Add new dialogue entry"))
        {
            string newDialogue = "";
            trigger.TriggerEvent.Dialogue.Add(newDialogue);
        }

        if (GUILayout.Button("Remove dialogue entry"))
        {
            trigger.TriggerEvent.Dialogue.Remove(trigger.TriggerEvent.Dialogue[trigger.TriggerEvent.Dialogue.Count - 1]);
        }

        EditorGUIUtility.labelWidth = 200f;
        trigEvent.ForceShow = (TriggerHandler.ForceOptions)EditorGUILayout.EnumPopup("Force Show?", trigEvent.ForceShow);
        trigEvent.ShowOnce = EditorGUILayout.Toggle("Show Once?", trigEvent.ShowOnce);
        trigEvent.ShowOnCompletedLevel = EditorGUILayout.Toggle("Show on Completed Level?", trigEvent.ShowOnCompletedLevel);
        trigEvent.ShowOnRestart = EditorGUILayout.Toggle("Show on Level Restart?", trigEvent.ShowOnRestart);
        trigEvent.HasDependency = EditorGUILayout.Toggle("Has Dependency?", trigEvent.HasDependency);

        if (trigEvent.HasDependency)
            trigEvent.DependencyId = (TriggerHandler.DependencyId)EditorGUILayout.EnumPopup("Dependency", trigEvent.DependencyId);
        
    }

    private void OnDestroy()
    {
        if (target != null) return;
        if (trigger == null) return;
        TriggerEventSerializer.Instance.RemoveTriggerEvent(trigger.TriggerId);
    }
}
