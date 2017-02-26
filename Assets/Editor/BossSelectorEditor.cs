using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossCreator))]
public class BossSelectorEditor : Editor {

    BossCreator bossProps;

    public override void OnInspectorGUI()
    {
        bossProps = (BossCreator)target;

        DisplayBossName();
        EditorGUILayout.Space();
        DisplayBossObjectDropdown();
        EditorGUILayout.Space();
        DisplayAbilitySet();
        
        EditorUtility.SetDirty(target);
    }

    private void DisplayBossName()
    {
        if (bossProps.BossName == null || bossProps.BossName == string.Empty)
            bossProps.BossName = EditorHelpers.AddSpacesToName(bossProps.name);

        bossProps.BossName = EditorGUILayout.TextField("Boss Name", bossProps.BossName);
    }

    private void DisplayBossObjectDropdown()
    {
        var bosses = Resources.LoadAll<GameObject>("NPCs/Bosses");

        if (bossProps.BossPrefab == null)
            bossProps.BossPrefab = bosses[0];

        var bossInspectorIndex = EditorHelpers.GetIndexFromObject(bosses, bossProps.BossPrefab);
        var bossArray = EditorHelpers.ObjectArrayToStringArray(bosses);

        int bossIndex = EditorGUILayout.Popup("Boss Prefab", bossInspectorIndex, bossArray);
        bossProps.BossPrefab = bosses[bossIndex];
    }

    private void DisplayAbilitySet()
    {
        var abilities = EditorHelpers.GetScriptAssetsOfType<BossAbility>();

        if (bossProps.AbilitySet == null || bossProps.AbilitySet.Length == 0)
        {
            bossProps.AbilitySet = new MonoScript[1];
            bossProps.AbilitySet[0] = abilities[0];
        }

        var abilityInspectorIndex = EditorHelpers.GetIndexFromObject(abilities, bossProps.AbilitySet[0]);   // TODO cycle through all
        var abilityArray = EditorHelpers.ObjectArrayToStringArray(abilities);

        EditorGUILayout.LabelField("Abilities");
        int abilityIndex = EditorGUILayout.Popup("Ability 1", abilityInspectorIndex, abilityArray);
        bossProps.AbilitySet[0] = abilities[abilityIndex];
    }

}
