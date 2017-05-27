using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StateMachine))]
public class BossSelectorEditor : Editor {

    private StateMachine machine;
    
    private bool bAttributesClicked;

    public override void OnInspectorGUI()
    {
        machine = (StateMachine)target;

        DisplayBossName();
        DisplayBossObjectDropdown();
        DisplayBossAttributes();

        EditorUtility.SetDirty(target);
    }

    private void DisplayBossName()
    {
        if (machine.BossName == null || machine.name == string.Empty)
            machine.BossName = BossSelectorHelpers.AddSpacesToName(machine.name);
        
        machine.BossName = EditorGUILayout.TextField("Boss Name", machine.BossName);
    }

    private void DisplayBossObjectDropdown()
    {
        var bosses = Resources.LoadAll<GameObject>("NPCs/Bosses");

        if (machine.BossPrefab == null)
            machine.BossPrefab = bosses[0];

        var bossInspectorIndex = BossSelectorHelpers.GetIndexFromObject(bosses, machine.BossPrefab);
        var bossArray = BossSelectorHelpers.ObjectArrayToStringArray(bosses);

        EditorGUILayout.Space();
        int bossIndex = EditorGUILayout.Popup("Boss Prefab", bossInspectorIndex, bossArray);
        machine.BossPrefab = bosses[bossIndex];
    }

    private void DisplayBossAttributes()
    {
        EditorGUILayout.Space();
        bAttributesClicked = EditorGUILayout.Foldout(bAttributesClicked, "Boss Attributes", true);
        machine.Health = EditorGUILayout.IntField("Boss Health:", machine.Health);
        machine.SpawnMoths = EditorGUILayout.Toggle("Spawns moths?", machine.SpawnMoths); // TODO Add dropdown for spawn type
        machine.ShakeScreenOnLanding = EditorGUILayout.Toggle("Shake on landing?", machine.ShakeScreenOnLanding);
    }

}
