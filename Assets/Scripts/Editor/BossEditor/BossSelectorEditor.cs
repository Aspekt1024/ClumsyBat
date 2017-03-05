using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossCreator))]
public class BossSelectorEditor : Editor {

    private BossCreator creatorObj;
    private readonly List<Type> abilities = new List<Type>();

    private bool bAbilitiesClicked;
    private bool bAttributesClicked;

    public override void OnInspectorGUI()
    {
        creatorObj = (BossCreator)target;
        GetAbilityList();

        DisplayBossName();
        EditorGUILayout.Space();
        DisplayBossObjectDropdown();
        EditorGUILayout.Space();
        DisplayBossAttributes();
        EditorGUILayout.Space();
        DisplayAbilitySet();
        EditorGUILayout.Space();
        DisplayStates();

        EditorUtility.SetDirty(target);
    }
    
    private void GetAbilityList()
    {
        foreach(var node in creatorObj.Actions)
        {
            Debug.Log("Node: " + node);
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

        int bossIndex = EditorGUILayout.Popup("Boss Prefab", bossInspectorIndex, bossArray);
        creatorObj.BossPrefab = bosses[bossIndex];
    }

    private void DisplayBossAttributes()
    {
        bAttributesClicked = EditorGUILayout.Foldout(bAttributesClicked, "Boss Attributes", true);
        if (bAttributesClicked)
        {
            creatorObj.Health = EditorGUILayout.IntField("Boss Health: ", creatorObj.Health);
            creatorObj.bSpawnMoths = EditorGUILayout.Toggle("Spawns moths?", creatorObj.bSpawnMoths); // TODO Add dropdown for spawn type
        }
    }

    private void DisplayAbilitySet()
    {
        var abilities = BossSelectorHelpers.GetScriptAssetsOfType<BossAbility>();

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
                EditorGUILayout.LabelField(string.Format("Ability {0}:{1}", i + 1, abilities[i].name));
                // TODO add ability attributes to abilities pane
            }
        }
        EditorGUILayout.Separator();

    }

    private void DisplayStates()
    {
        EditorGUILayout.LabelField("Boss States:");
        for (int i = 0; i < creatorObj.States.Count; i++)
        {
            creatorObj.States[i] = (BossState)EditorGUILayout.ObjectField("State " + i, creatorObj.States[i], typeof(BossState), true);
        }
    }

}
