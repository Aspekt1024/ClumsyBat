using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossCreator))]
public class BossSelectorEditor : Editor {

    private BossCreator bossProps;
    private readonly List<Type> abilities = new List<Type>();

    private bool bAbilitiesClicked;
    private bool bAttributesClicked;

    public override void OnInspectorGUI()
    {
        bossProps = (BossCreator)target;
        GetAbilityList();

        DisplayBossName();
        EditorGUILayout.Space();
        DisplayBossObjectDropdown();
        EditorGUILayout.Space();
        DisplayBossAttributes();
        EditorGUILayout.Space();
        DisplayAbilitySet();
        
        EditorUtility.SetDirty(target);
    }
    
    private void GetAbilityList()
    {
        foreach(var node in bossProps.Nodes)
        {
            if (node.GetType().Equals(typeof(JumpNode)))
                AddAbility<JumpPound>();
            if (node.GetType().Equals(typeof(ParabolicProjectileNode)))
                AddAbility<ParabolicProjectile>();
        }
    }

    private void AddAbility<T>() where T : BossAbility
    {
        bool abilityExitsInList = false;
        foreach (Type ability in abilities)
        {
            if (ability.Equals(typeof(T)))
            {
                abilityExitsInList = true;
                break;
            }
        }
        if (!abilityExitsInList)
        {
            abilities.Add(typeof(T));
        }
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

    private void DisplayBossAttributes()
    {
        bAttributesClicked = EditorGUILayout.Foldout(bAttributesClicked, "Boss Attributes", true);
        if (bAbilitiesClicked)
        {
            bossProps.Health = EditorGUILayout.IntField("Boss Health: ", bossProps.Health);
            bossProps.bSpawnMoths = EditorGUILayout.Toggle("Spawns moths?", bossProps.bSpawnMoths); // TODO Add dropdown for spawn type
        }
    }

    private void DisplayAbilitySet()
    {
        var abilities = EditorHelpers.GetScriptAssetsOfType<BossAbility>();

        EditorGUILayout.Separator();
        GUIContent someContent = new GUIContent()
        {
            text = "Boss Abilities",
            image = Resources.Load<Texture>("LevelButtons/Main1Available"),
        };
        bAbilitiesClicked = EditorGUILayout.Foldout(bAbilitiesClicked, someContent, true);
        if (bAbilitiesClicked)
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                EditorGUILayout.LabelField("Ability " + (i + 1) + ":" + abilities[i].name);
                // TODO add ability attributes to abilities pane
            }
        }
        EditorGUILayout.Separator();

    }

}
