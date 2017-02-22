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

        bossProps.BossName = EditorGUILayout.TextField("Boss Name", bossProps.BossName);
        EditorGUILayout.Space();
        DisplayBossObjectDropdown();
        EditorGUILayout.Space();
        DisplayAbilitySet();
    }

    private void DisplayBossObjectDropdown()
    {
        var bosses = EditorHelpers.GetScriptAssetsOfType<Boss>();
        var bossInspectorIndex = EditorHelpers.GetIndexFromObject(bosses, bossProps.BossObject);
        var bossArray = EditorHelpers.ObjectArrayToStringArray(bosses);

        int bossIndex = EditorGUILayout.Popup("Boss Script", bossInspectorIndex, bossArray);
        bossProps.BossObject = bosses[bossIndex];
    }

    private void DisplayAbilitySet()
    {
        var abilities = EditorHelpers.GetScriptAssetsOfType<BossAbility>();

        if (bossProps.AbilitySet.Length == 0)
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
