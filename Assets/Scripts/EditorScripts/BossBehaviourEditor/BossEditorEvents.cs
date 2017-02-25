using UnityEngine;
public class BossEditorEvents {

    public delegate void BossEditorEventHandler(BossEditorMouseInput.NodeTypes nodeType);
    public static BossEditorEventHandler OnCreateNode;

    public static void CreateNode(BossEditorMouseInput.NodeTypes nodeType)
    {
        if (OnCreateNode != null)
            OnCreateNode(nodeType);
    }
}
