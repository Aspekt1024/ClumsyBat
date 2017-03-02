using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

using NodeMenuSelections = BossEditorMouseInput.NodeMenuSelections;

public class BossEditorContextMenus{
    
    private BossEditor editor;
    private BaseNode selectedNode;

    public BossEditorContextMenus(BossEditor editorInstance)
    {
        editor = editorInstance;
    }

    public void ShowMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("JumpPound/Jump"), false, ContextCallback, typeof(JumpNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Death Node"), false, ContextCallback, typeof(BaseNode));
        menu.AddItem(new GUIContent("Add Wait Node"), false, ContextCallback, typeof(WaitNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Start Node"), false, ContextCallback, typeof(StartNode));
        menu.AddItem(new GUIContent("Add Loop Node"), false, ContextCallback, typeof(LoopNode));
        menu.ShowAsContext();
    }
    
    public void ShowNodeMenu(BaseNode mouseDownNode)
    {
        selectedNode = mouseDownNode;

        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Do Nothing"), false, ContextCallback, NodeMenuSelections.DoNothing);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, NodeMenuSelections.DeleteNode);
        menu.ShowAsContext();
    }
    
    public void ContextCallback(object obj)
    {
        if (obj is Type)
            CreateNodeIfNodeType((Type)obj);
        else if (obj is NodeMenuSelections)
            ActionNodeMenuSelection((NodeMenuSelections)obj);
    }

    private void CreateNodeIfNodeType(Type type)
    {
        if (!type.IsSubclassOf(typeof(BaseNode))) return;

        MethodInfo method = ((Action)CreateNode<BaseNode>)
            .Method
            .GetGenericMethodDefinition()
            .MakeGenericMethod(type);

        method.Invoke(this, null);
    }

    public void CreateNode<T>() where T : BaseNode
    {
        if (typeof(T).Equals(typeof(StartNode)) && editor.StartExists())
        {
            Debug.LogError("Start Node already exists. Cannot create another!");
            return;
        }

        BaseNode newNode = ScriptableObject.CreateInstance<T>();
        if (newNode != null)
            editor.AddNode(newNode);
    }

    private void ActionNodeMenuSelection(NodeMenuSelections selection)
    {
        switch (selection)
        {
            case NodeMenuSelections.DoNothing:
                // as it says
                break;
            case NodeMenuSelections.DeleteNode:
                editor.RemoveNode(selectedNode);
                break;
        }
    }
}
