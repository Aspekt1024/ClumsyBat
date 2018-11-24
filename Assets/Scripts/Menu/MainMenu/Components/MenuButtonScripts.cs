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
            GameStatics.UI.MainMenuTransitions.AnimateToLevelSelect();
        }

        public void OptionsButtonPressed()
        {
            StartCoroutine(ShowDropdownRoutine(GameStatics.UI.DropdownMenu.ShowOptions));
        }

        public void StatsButtonPressed()
        {
            StartCoroutine(ShowDropdownRoutine(GameStatics.UI.DropdownMenu.ShowStats));
        }

        private IEnumerator ShowDropdownRoutine(System.Action action)
        {
            GameStatics.UI.MainMenuTransitions.GotoDropdownArea();
            yield return new WaitForSeconds(0.7f);
            action.Invoke();
        }
    }
}
