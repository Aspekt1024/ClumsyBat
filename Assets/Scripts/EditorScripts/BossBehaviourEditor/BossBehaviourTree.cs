using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[CreateAssetMenu(fileName = "BossBehaviour", menuName = "Custom/Boss Behaviour", order = 1)]
public class BossBehaviourTree : ScriptableObject {

    public enum BossActions
    {
        Wait,
        Speak,
        Die
    }

    public List<BossActions> things;

    [OnOpenAssetAttribute(1)]
    private static bool Step1(int instanceID, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);
        if (EditorUtility.InstanceIDToObject(instanceID).GetType() == typeof(BossBehaviourTree))
        {
            BossEditor editor = EditorWindow.GetWindow<BossEditor>(typeof(SceneView));
            editor.BossBehaviourObject = (BossBehaviourTree)obj;
        }
        return false;
    }

}
