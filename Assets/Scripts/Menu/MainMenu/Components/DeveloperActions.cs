using ClumsyBat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeveloperActions : MonoBehaviour {
    
    public void UnlockAllLevels()
    {
        GameStatics.Data.LevelDataHandler.UnlockAllLevels();
        Toolbox.Instance.MenuScreen = Toolbox.MenuSelector.LevelSelect;
        SceneManager.LoadScene("Play");
    }

    public void UnlockAllAbilities()
    {
        GameStatics.Data.Abilities.ActivateAllAbilities();
    }
}
