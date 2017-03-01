using UnityEngine;
public class BossEditorEvents {

    public delegate void BossEditorEventHandler(BossNodeFactory.NodeTypes nodeType);
    public static BossEditorEventHandler OnCreateNode;


    public static void CreateNode(BossNodeFactory.NodeTypes nodeType)
    {
        if (OnCreateNode != null)
            OnCreateNode(nodeType);
    }
}
