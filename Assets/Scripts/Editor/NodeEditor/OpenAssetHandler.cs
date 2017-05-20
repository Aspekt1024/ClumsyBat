using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public static class OpenAssetHandler {
    
    [OnOpenAsset(1)]
    private static bool LoadBossEditor(int instanceID, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);
        if (EditorUtility.InstanceIDToObject(instanceID).GetType() == typeof(BossStateMachine))
        {
            BossEditor editor = EditorWindow.GetWindow<BossEditor>(desiredDockNextTo: typeof(SceneView));
            editor.LoadEditor((BossStateMachine)obj);
        }
        return false;
    }
    
    [OnOpenAsset(2)]
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
