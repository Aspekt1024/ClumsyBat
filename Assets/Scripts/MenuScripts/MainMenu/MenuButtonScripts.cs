using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonScripts : MonoBehaviour {

    public void ScoresButtonPressed()
    {
        PlayGamesScript.ShowLeaderboardsUI();
    }
}
