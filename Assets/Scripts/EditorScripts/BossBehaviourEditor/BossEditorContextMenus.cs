using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class BossEditorContextMenus{
    
    private BossEditor editor;
    private BaseNode selectedNode;
    
    public enum NodeMenuSelections
    {
        FindStart,
        DeleteNode
    }

    public BossEditorContextMenus(BossEditor editorInstance)
    {
        editor = editorInstance;
    }

    public void ShowMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("JumpPound/Jump"), false, ContextCallback, typeof(JumpNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Death Node"), false, ContextCallback, typeof(BaseNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Wait Node"), false, ContextCallback, typeof(WaitNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Start Node"), false, ContextCallback, typeof(StartNode));
        menu.AddItem(new GUIContent("Add Loop Node"), false, ContextCallback, typeof(LoopNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Find Start"), false, ContextCallback, NodeMenuSelections.FindStart);
        menu.ShowAsContext();
    }
    
    public void ShowNodeMenu(BaseNode mouseDownNode)
    {
        selectedNode = mouseDownNode;

        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Find Start"), false, ContextCallback, NodeMenuSelections.FindStart);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, NodeMenuSelections.DeleteNode);
        menu.ShowAsContext();
    }


    private void ActionNodeMenuSelection(NodeMenuSelections selection)
    {
        switch (selection)
        {
            case NodeMenuSelections.FindStart:
                editor.MoveToStart();
                break;
            case NodeMenuSelections.DeleteNode:
                editor.RemoveNode(selectedNode);
                break;
        }
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
}
