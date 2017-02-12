using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AbilityControl : MonoBehaviour {

    private AbilityContainer.AbilityDataType _abilities;

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/AbilityData.dat"))
        {
            var bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/AbilityData.dat", FileMode.Open);

            AbilityContainer abilityData;
            try
            {
                abilityData = (AbilityContainer)bf.Deserialize(file);
            }
            catch
            {
                abilityData = new AbilityContainer();
                Debug.Log("Unable to load existing ability data set");
            }
            file.Close();

            _abilities = abilityData.Data;
        }

        if (!_abilities.AbilitiesCreated)
        {
            InitialiseAbilities();
        }
    }

    private void InitialiseAbilities()
    {
        _abilities.Rush = NewAbility();
        _abilities.Hypersonic = NewAbility();
        _abilities.LanternDurationLevel = NewAbility();
        _abilities.AbilitiesCreated = true;
        Save();
        Debug.Log("New abilitity data set saved");
    }

    private static AbilityContainer.AbilityType NewAbility()
    {
        var ability = new AbilityContainer.AbilityType
        {
            AbilityLevel = 1,
            AbilityEvolution = 1,
            AbilityAvailable = false,
            AbilityUnlocked = false,
            UpgradeAvailable = false
        };
        return ability;
    }

    public void Save()
    {
        var bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/AbilityData.dat", FileMode.OpenOrCreate);

        AbilityContainer abilityData = new AbilityContainer { Data = _abilities };

        bf.Serialize(file, abilityData);
        file.Close();
    }

    public void ClearAbilityData()
    {
        var bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/AbilityData.dat", FileMode.Create);

        var blankAbilityData = new AbilityContainer { Data = new AbilityContainer.AbilityDataType() };

        bf.Serialize(file, blankAbilityData);
        file.Close();
        Load();
        Debug.Log("Ability Data Cleared");
    }

    public AbilityContainer.AbilityType GetRushStats() { return _abilities.Rush; }
    public void SaveRushStats(AbilityContainer.AbilityType rush) { _abilities.Rush = rush; }

    public AbilityContainer.AbilityType GetHypersonicStats() { return _abilities.Hypersonic; }
    public void SaveHypersonicStats(AbilityContainer.AbilityType hyperSonic) { _abilities.Hypersonic = hyperSonic; }

    public AbilityContainer.AbilityType GetLanternDurationStats() { return _abilities.LanternDurationLevel; }
    public void SaveLanternDurationStats(AbilityContainer.AbilityType lanternDurationLevel) { _abilities.LanternDurationLevel = lanternDurationLevel; }
}

[Serializable]
public class AbilityContainer
{
    [Serializable]
    public struct AbilityType
    {
        public int AbilityLevel;
        public int AbilityEvolution;
        public bool AbilityUnlocked;
        public bool AbilityAvailable;
        public bool UpgradeAvailable;
    }

    [Serializable]
    public struct AbilityDataType
    {
        public bool AbilitiesCreated;
        public AbilityType Rush;
        public AbilityType Hypersonic;
        public AbilityType LanternDurationLevel;
        // Lantern view distance
        // Moth Magnet
        // Moth value
    }
    public AbilityDataType Data;
}