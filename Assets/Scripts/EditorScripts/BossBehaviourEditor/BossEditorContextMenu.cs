using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BossEditorContextMenu{

	public static GenericMenu GetMenu(GenericMenu.MenuFunction2 callbackFunc)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Speech Node"), false, callbackFunc, BossEditor.NodeTypes.SaySomething);
        menu.AddItem(new GUIContent("JumpPound/Activate"), false, callbackFunc, BossEditor.NodeTypes.Jump);
        menu.AddItem(new GUIContent("Add Death Node"), false, callbackFunc, BossEditor.NodeTypes.Die);
        
        return menu;
    }
    
    public static GenericMenu GetNodeMenu(GenericMenu.MenuFunction2 callbackFunc)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Do Nothing"), false, callbackFunc, BossEditor.NodeMenuSelections.DoNothing);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Node"), false, callbackFunc, BossEditor.NodeMenuSelections.DeleteNode);
        return menu;
    }
}
