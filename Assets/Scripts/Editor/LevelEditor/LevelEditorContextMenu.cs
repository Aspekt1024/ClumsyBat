using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditorContextMenu {

    private LevelEditor editor;
    private LevelEditorActions actions;

    public LevelEditorContextMenu(LevelEditor editorRef, LevelEditorActions actionsRef)
    {
        editor = editorRef;
        actions = actionsRef;
    }

    public void ShowMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Moth (shift + 1)"), false, ContextCallback, typeof(MothEditorHandler));
        menu.AddItem(new GUIContent("Spike (shift + 2)"), false, ContextCallback, typeof(SpiderEditorHandler));
        menu.AddItem(new GUIContent("Mushroom (shift + 3)"), false, ContextCallback, typeof(MushroomEditorHandler));
        menu.AddItem(new GUIContent("Spider (shift + 4)"), false, ContextCallback, typeof(SpiderEditorHandler));
        menu.AddItem(new GUIContent("Web (shift + 5)"), false, ContextCallback, typeof(WebEditorHandler));
        menu.AddItem(new GUIContent("Trigger (shift + 6)"), false, ContextCallback, typeof(TriggerEditorHandler));
        menu.ShowAsContext();
    }

    private void ContextCallback(object obj)
    {
        if (!((Type)obj).IsSubclassOf(typeof(BaseObjectHandler))) return;
        editor.HeldObject = actions.ObjectHandler.SpawnObject(obj);
    }


}
