﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using NodeTypes = BossEditorMouseInput.NodeTypes;

public class BossNodeFactory {

    private BossEditor _editor;

    public BossNodeFactory(BossEditor editor)
    {
        _editor = editor;
        BossEditorEvents.OnCreateNode += CreateNode;
    }
    ~BossNodeFactory()
    {
        BossEditorEvents.OnCreateNode -= CreateNode;
    }
    
    private void CreateNode(NodeTypes nodeType)
    {
        BaseNode newNode = null;
        switch (nodeType)
        {
            case NodeTypes.Start:
                //inputNode = CreateInstance<StartNode>(); // TODO this
                break;
            case NodeTypes.End:
                //inputNode = CreateInstance<EndNode>(); // TODO this
                break;
            case NodeTypes.SaySomething:
                newNode = ScriptableObject.CreateInstance<SpeechNode>();
                break;
            case NodeTypes.Jump:
                newNode = ScriptableObject.CreateInstance<BaseNode>();
                break;
            case NodeTypes.Die:
                newNode = ScriptableObject.CreateInstance<BaseNode>();
                break;
        }
        if (newNode != null)
        {
            _editor.AddNode(newNode);
        }
    }

}
