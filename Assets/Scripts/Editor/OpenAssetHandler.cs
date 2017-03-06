using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public static class OpenAssetHandler {
    
    [OnOpenAsset()]
    private static bool LoadBossEditor(int instanceID, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);
        if (EditorUtility.InstanceIDToObject(instanceID).GetType() == typeof(BossCreator))
        {
            BossEditor editor = EditorWindow.GetWindow<BossEditor>(desiredDockNextTo: typeof(SceneView));
            editor.LoadEditor((BossCreator)obj);
        }
        return false;
    }
    
    [OnOpenAsset()]
    private static bool LoadBossStateEditor(int instanceID, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);
        if (EditorUtility.InstanceIDToObject(instanceID).GetType() == typeof(BossState))
        {
            BossStateEditor editor = EditorWindow.GetWindow<BossStateEditor>(desiredDockNextTo: typeof(SceneView));
            editor.LoadEditor((BossState)obj);
        }
        return false;
    }
}
