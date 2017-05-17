using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossCreator))]
public class BossSelectorEditor : Editor {

    private BossCreator creatorObj;

    private bool bAbilitiesClicked;
    private bool bAttributesClicked;

    public override void OnInspectorGUI()
    {
        creatorObj = (BossCreator)target;

        DisplayBossName();
        DisplayBossObjectDropdown();
        DisplayBossAttributes();

        EditorUtility.SetDirty(target);
    }

    private void DisplayBossName()
    {
        if (creatorObj.BossName == null || creatorObj.BossName == string.Empty)
            creatorObj.BossName = BossSelectorHelpers.AddSpacesToName(creatorObj.name);
        
        creatorObj.BossName = EditorGUILayout.TextField("Boss Name", creatorObj.BossName);
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
