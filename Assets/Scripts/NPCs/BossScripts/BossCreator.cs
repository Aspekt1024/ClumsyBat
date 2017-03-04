using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossData", menuName = "Custom/Boss", order = 1)]
public class BossCreator : ScriptableObject
{
    public string BossName;
    public GameObject BossPrefab; 
    public int Health;
    public bool bSpawnMoths;    // TODO make into selectable list, per state
    
    public List<BaseNode> Nodes = new List<BaseNode>();
    public List<BossState> States = new List<BossState>();

    public BossState CurrentState;

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

    public void NodeGameSetup(BossBehaviour behaviour, GameObject boss)
    {
        foreach (var state in States)
        {
            foreach (var node in state.Nodes)
            {
                node.GameSetup(behaviour, boss);
            }
        }
    }

    public void AwakenBoss()
    {
        foreach(var node in Nodes)
        {
            if (node.GetType().Equals(typeof(StartNode)))
                ActivateStateIfStateNode(node.GetNextNode());
        }
    }

    private void ActivateStateIfStateNode(BaseNode node)
    {
        if (!node.GetType().Equals(typeof(StateNode))) return;

        CurrentState = ((StateNode)node).State;

        foreach (var n in CurrentState.Nodes)
        {
            if (n.GetType().Equals(typeof(StartNode)))
                n.Activate();
        }
    }
}
