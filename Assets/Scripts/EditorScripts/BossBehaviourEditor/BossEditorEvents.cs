using UnityEngine;
public class BossEditorEvents{

    public delegate void BossEditorEventHandler<T>() where T : BaseNode;
    public static BossEditorEventHandler<BaseNode> OnCreateNode;
    
    public static void CreateNode<T>()
    {
        if (OnCreateNode != null)
            OnCreateNode();
    }
}
