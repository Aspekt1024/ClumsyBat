using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[CreateAssetMenu(fileName = "BossData", menuName = "Custom/Boss State", order = 1)]
public class BossState : ScriptableObject {

    public string BossName = "<BossName>";
    public string StateName = "State";
    public List<BaseNode> Nodes = new List<BaseNode>();

    [OnOpenAsset()]
    private static bool LoadBossEditor(int instanceID, int line)
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
