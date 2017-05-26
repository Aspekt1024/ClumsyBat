using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StateMachine))]
public class BossSelectorEditor : Editor {

    private StateMachine creatorObj;
    
    private bool bAttributesClicked;

    public override void OnInspectorGUI()
    {
        creatorObj = (StateMachine)target;

        DisplayBossName();
        DisplayBossObjectDropdown();
        DisplayBossAttributes();

        EditorUtility.SetDirty(target);
    }

    private void DisplayBossName()
    {
        if (creatorObj.Name == null || creatorObj.Name == string.Empty)
            creatorObj.Name = BossSelectorHelpers.AddSpacesToName(creatorObj.name);
        
        creatorObj.Name = EditorGUILayout.TextField("Boss Name", creatorObj.Name);
    }

    private void DisplayBossObjectDropdown()
    {
        var bosses = Resources.LoadAll<GameObject>("NPCs/Bosses");

        if (creatorObj.BossPrefab == null)
            creatorObj.BossPrefab = bosses[0];

        var bossInspectorIndex = BossSelectorHelpers.GetIndexFromObject(bosses, creatorObj.BossPrefab);
        var bossArray = BossSelectorHelpers.ObjectArrayToStringArray(bosses);

        EditorGUILayout.Space();
        int bossIndex = EditorGUILayout.Popup("Boss Prefab", bossInspectorIndex, bossArray);
        creatorObj.BossPrefab = bosses[bossIndex];
    }

    private void DisplayBossAttributes()
    {
        EditorGUILayout.Space();
        bAttributesClicked = EditorGUILayout.Foldout(bAttributesClicked, "Boss Attributes", true);
        creatorObj.Health = EditorGUILayout.IntField("Boss Health:", creatorObj.Health);
        creatorObj.SpawnMoths = EditorGUILayout.Toggle("Spawns moths?", creatorObj.SpawnMoths); // TODO Add dropdown for spawn type
        creatorObj.ShakeScreenOnLanding = EditorGUILayout.Toggle("Shake on landing?", creatorObj.ShakeScreenOnLanding);
    }

}
