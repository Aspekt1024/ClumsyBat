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
            FileStream file = File.Open(Application.persistentDataPath + "/AbilityData.dat", FileMode.Open);
            AbilityContainer AbilityData = (AbilityContainer)bf.Deserialize(file);
            file.Close();

            Abilities = AbilityData.Data;
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/AbilityData.dat", FileMode.OpenOrCreate);

        AbilityContainer AbilityData = new AbilityContainer();
        AbilityData.Data = Abilities;

        bf.Serialize(file, AbilityData);
        file.Close();
    }

    public void ClearCompletionData()
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

    public AbilityContainer.AbilityType GetLanternDurationStats() { return Abilities.LanternDuration; }
    public void SaveLanternDurationStats(AbilityContainer.AbilityType LanternDuration) { Abilities.Rush = LanternDuration; }
}

[Serializable]
public class AbilityContainer
{
    [Serializable]
    public struct AbilityType
    {
        public bool AbilityUnlocked;
        public int AbilityLevel;
    }

    [Serializable]
    public struct AbilityDataType
    {
        public AbilityType Rush;
        public AbilityType Hypersonic;
        public AbilityType LanternDuration;
        // Lantern view distance
        // Moth Magnet
        // Moth value
    }
    public AbilityDataType Data;
}