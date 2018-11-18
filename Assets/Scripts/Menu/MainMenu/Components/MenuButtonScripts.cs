using System.Collections;
using UnityEngine;

namespace ClumsyBat.Menu
{
    public class MenuButtonScripts : MonoBehaviour
    {

        public void ScoresButtonPressed()
        {
            PlayGamesScript.ShowLeaderboardsUI();
        }

        public void PlayButtonPressed()
        {
            var transitions = new MainMenuTransitions();
            transitions.AnimateToLevelSelect();
        }

        public void OptionsButtonPressed()
        {

        }

        public void StatsButtonPressed()
        {

        }
    }
}
