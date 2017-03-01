using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BossNodeFactory {
    
    // TODO auto search nodetypes based on inheritance tree?
    public enum NodeTypes
    {
        Start,
        End,
        SaySomething,
        Jump,
        Die,
        Wait,
        Loop
    }

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
        if (_editor == null) return;    // occurs when this object hasn't been destroyed when the editor hits OnDisable()

        BaseNode newNode = null;
        switch (nodeType)
        {
            case NodeTypes.Start:
                newNode = ScriptableObject.CreateInstance<StartNode>();
                break;
            case NodeTypes.End:
                newNode = ScriptableObject.CreateInstance<EndNode>();
                break;
            case NodeTypes.SaySomething:
                newNode = ScriptableObject.CreateInstance<SpeechNode>();
                break;
            case NodeTypes.Jump:
                newNode = ScriptableObject.CreateInstance<JumpNode>();
                break;
            case NodeTypes.Die:
                newNode = ScriptableObject.CreateInstance<BaseNode>();
                break;
            case NodeTypes.Wait:
                newNode = ScriptableObject.CreateInstance<WaitNode>();
                break;
            case NodeTypes.Loop:
                newNode = ScriptableObject.CreateInstance<LoopNode>();
                break;
        }
        if (newNode != null)
        {
            _editor.AddNode(newNode);
        }
    }

}
