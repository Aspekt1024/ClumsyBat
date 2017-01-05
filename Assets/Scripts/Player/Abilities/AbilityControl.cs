using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AbilityControl : MonoBehaviour {

    private AbilityContainer.AbilityDataType Abilities = new AbilityContainer.AbilityDataType();

    void Start ()
    {
	
	}
	

	void Update ()
    {
	
	}

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/AbilityData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file;
            file = File.Open(Application.persistentDataPath + "/AbilityData.dat", FileMode.Open);

            AbilityContainer AbilityData;
            try
            {
                AbilityData = (AbilityContainer)bf.Deserialize(file);
            }
            catch
            {
                AbilityData = new AbilityContainer();
                Debug.Log("Unable to load existing ability data set");
            }
            file.Close();

            Abilities = AbilityData.Data;
        }

        if (!Abilities.AbilitiesCreated)
        {
            InitialiseAbilities();
        }
    }

    private void InitialiseAbilities()
    {
        Abilities.Rush = NewAbility();
        Abilities.Hypersonic = NewAbility();
        Abilities.LanternDurationLevel = NewAbility();
        Abilities.AbilitiesCreated = true;
        Save();
        Debug.Log("New abilitity data set saved");
    }

    private AbilityContainer.AbilityType NewAbility()
    {
        AbilityContainer.AbilityType Ability = new AbilityContainer.AbilityType();
        Ability.AbilityLevel = 1;
        Ability.AbilityEvolution = 1;
        Ability.AbilityAvailable = false;
        Ability.AbilityUnlocked = false;
        Ability.UpgradeAvailable = false;
        return Ability;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        file = File.Open(Application.persistentDataPath + "/AbilityData.dat", FileMode.OpenOrCreate);

        AbilityContainer AbilityData = new AbilityContainer();
        AbilityData.Data = Abilities;

        bf.Serialize(file, AbilityData);
        file.Close();
    }

    public void ClearAbilityData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/AbilityData.dat", FileMode.Create);

        AbilityContainer BlankAbilityData = new AbilityContainer();
        BlankAbilityData.Data = new AbilityContainer.AbilityDataType();

        bf.Serialize(file, BlankAbilityData);
        file.Close();
        Load();
        Debug.Log("Ability Data Cleared");
    }

    public AbilityContainer.AbilityType GetRushStats() { return Abilities.Rush; }
    public void SaveRushStats(AbilityContainer.AbilityType Rush) { Abilities.Rush = Rush; }

    public AbilityContainer.AbilityType GetHypersonicStats() { return Abilities.Hypersonic; }
    public void SaveHypersonicStats(AbilityContainer.AbilityType HyperSonic) { Abilities.Hypersonic = HyperSonic; }

    public AbilityContainer.AbilityType GetLanternDurationStats() { return Abilities.LanternDurationLevel; }
    public void SaveLanternDurationStats(AbilityContainer.AbilityType LanternDurationLevel) { Abilities.LanternDurationLevel = LanternDurationLevel; }
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