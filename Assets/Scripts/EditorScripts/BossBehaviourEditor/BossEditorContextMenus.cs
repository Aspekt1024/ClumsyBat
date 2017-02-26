using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using NodeTypes = BossEditorMouseInput.NodeTypes;
using NodeMenuSelections = BossEditorMouseInput.NodeMenuSelections;

public static class BossEditorContextMenus{

	public static void ShowMenu(GenericMenu.MenuFunction2 callbackFunc)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Start Node"), false, callbackFunc, NodeTypes.Start);
        menu.AddItem(new GUIContent("Add End Node"), false, callbackFunc, NodeTypes.End);
        menu.AddItem(new GUIContent("Add Speech Node"), false, callbackFunc, NodeTypes.SaySomething);
        menu.AddItem(new GUIContent("JumpPound/Activate"), false, callbackFunc, NodeTypes.Jump);
        menu.AddItem(new GUIContent("Add Death Node"), false, callbackFunc, NodeTypes.Die);
        menu.ShowAsContext();
    }
    
    public static void ShowNodeMenu(GenericMenu.MenuFunction2 callbackFunc)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Do Nothing"), false, callbackFunc, NodeMenuSelections.DoNothing);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Node"), false, callbackFunc, NodeMenuSelections.DeleteNode);
        menu.ShowAsContext();
    }
}
