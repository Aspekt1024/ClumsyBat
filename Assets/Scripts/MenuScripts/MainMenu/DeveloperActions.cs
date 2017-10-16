using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeveloperActions : MonoBehaviour {

    private LevelDataControl levelDataControl;

    private void Start()
    {
        levelDataControl = GameData.Instance.Data.LevelData;
    }

    public void UnlockAllLevels()
    {
        levelDataControl.UnlockAllLevels();
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.LevelSelect;
        SceneManager.LoadScene("Play");
    }

    public void UnlockAllAbilities()
    {
        GameData.Instance.Data.AbilityData.ActivateAllAbilities();
    }
}
