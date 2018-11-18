using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ClumsyBat.DataContainers;

public class AbilityControl : MonoBehaviour {

    public AbilityContainer.AbilityDataType Data;
    private string persistentDataPath; // Can only be obtained in the main thread. Load/Save is async

    public void Awake()
    {
        persistentDataPath = Application.persistentDataPath;
    }

    public bool Load()
    {
        if (File.Exists(persistentDataPath + "/AbilityData.dat"))
        {
            var bf = new BinaryFormatter();
            FileStream file = File.Open(persistentDataPath + "/AbilityData.dat", FileMode.Open);

            AbilityContainer abilityData;
            try
            {
                abilityData = (AbilityContainer)bf.Deserialize(file);
            }
            catch
            {
                abilityData = new AbilityContainer();
                Debug.Log("Unable to load existing ability data set");
                return false;
            }
            file.Close();

            Data = abilityData.Data;
        }

        if (!Data.AbilitiesCreated)
        {
            InitialiseAbilities();
        }
        return true;
    }

    private void InitialiseAbilities()
    {
        Data.Rush = NewAbility();
        Data.Hypersonic = NewAbility();
        Data.LanternDurationLevel = NewAbility();
        Data.Shield = NewAbility();
        Data.AbilitiesCreated = true;
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

        FileStream file = File.Open(persistentDataPath + "/AbilityData.dat", FileMode.OpenOrCreate);

        AbilityContainer abilityData = new AbilityContainer { Data = Data };

        bf.Serialize(file, abilityData);
        file.Close();
    }

    public void ClearAbilityData()
    {
        var bf = new BinaryFormatter();
        FileStream file = File.Open(persistentDataPath + "/AbilityData.dat", FileMode.Create);

        var blankAbilityData = new AbilityContainer { Data = new AbilityContainer.AbilityDataType() };

        bf.Serialize(file, blankAbilityData);
        file.Close();
        Load();
        Debug.Log("Ability Data Cleared");
    }

    public void ActivateAllAbilities()
    {
        ActivateDash();
        ActivateHypersonic();
    }

    public void ActivateDash()
    {
        Data.Rush.AbilityAvailable = true;
        Data.Rush.AbilityUnlocked = true;
        Save();
    }

    public void ActivateHypersonic()
    {
        Data.Hypersonic.AbilityAvailable = true;
        Data.Hypersonic.AbilityUnlocked = true;
        Save();
    }

    public AbilityContainer.AbilityType GetDashStats() { return Data.Rush; }
    public void SaveDashStats(AbilityContainer.AbilityType rush) { Data.Rush = rush; }

    public AbilityContainer.AbilityType GetHypersonicStats() { return Data.Hypersonic; }
    public void SaveHypersonicStats(AbilityContainer.AbilityType hyperSonic) { Data.Hypersonic = hyperSonic; }

    public AbilityContainer.AbilityType GetLanternDurationStats() { return Data.LanternDurationLevel; }
    public void SaveLanternDurationStats(AbilityContainer.AbilityType lanternDurationLevel) { Data.LanternDurationLevel = lanternDurationLevel; }

    public AbilityContainer.AbilityType GetShieldStats() { return Data.Shield; }
    public void SaveShieldStats(AbilityContainer.AbilityType shieldStats) { Data.Shield = shieldStats; }
}
