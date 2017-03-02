using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossData", menuName = "Custom/Boss", order = 1)]
public class BossCreator : ScriptableObject
{
    public string BossName;
    public GameObject BossPrefab; 
    public MonoScript[] AbilitySet;
    
    public enum BossActions
    {
        Wait,
        Speak,
        Die
    }
    public List<BossActions> things;
    public List<BaseNode> Nodes;

    [OnOpenAsset()]
    private static bool LoadBossEditor(int instanceID, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);
        if (EditorUtility.InstanceIDToObject(instanceID).GetType() == typeof(BossCreator))
        {
            BossEditor editor = EditorWindow.GetWindow<BossEditor>(desiredDockNextTo: typeof(SceneView));
            editor.LoadBoss((BossCreator)obj);
        }
        return false;
    }

    // TODO on startup, cycle through each node and add abilities to the boss
    // Maybe also tell the node who the boss is? That way we can set references to the abilities
    
    public void AwakenBoss()
    {
        foreach(var node in Nodes)
        {
            if (node.WindowTitle == "Start")
            {
                node.Activate();
            }
        }
    }
}
