using UnityEngine;
using System.Collections;

namespace ClumsyBat.Menu
{
    public class MainMenuTransitions
    {
        private KeyPointsHandler keyPoints;

        public MainMenuTransitions()
        {
            keyPoints = Object.FindObjectOfType<KeyPointsHandler>();
        }

        public void AnimateMainMenuScene()
        {
            PlayerManager.Instance.PossessByAI();
            PlayerManager.Instance.SetPlayerPosition(keyPoints.EntryPoint.transform.position);
            PlayerManager.Instance.AIController.MoveTo(keyPoints.EntryLandingPoint.transform);
        }

        public void AnimateToLevelSelect()
        { 
            PlayerManager.Instance.PossessByAI();
            PlayerManager.Instance.AIController.MoveTo(keyPoints.MainMenuCamPoint.transform, AnimateThroughCave);
        }

        public void AnimateToMainMenu()
        {
            GameStatics.GameManager.StartCoroutine(ScrollToMainMenuRoutine());
        }

        private void AnimateThroughCave()
        {
            GameStatics.GameManager.StartCoroutine(ThroughCaveRoutine());
        }

        private IEnumerator ThroughCaveRoutine()
        {
            PlayerManager.Instance.AIController.DashTo(keyPoints.MainMenuExitPoint.transform);
            yield return new WaitForSeconds(0.3f);

            float endPosX = keyPoints.LevelMapStart.transform.position.x;
            GameStatics.Camera.SetEndPoint(endPosX);
            GameStatics.Camera.StartFollowing(keyPoints.LevelMapStart.transform, 2.9f);
            yield return new WaitForSeconds(1.2f);

            PlayerManager.Instance.SetPlayerPosition(keyPoints.LevelEntryPoint.transform.position);
            PlayerManager.Instance.AIController.Dash();
            yield return new WaitForSeconds(0.7f);

            PlayerManager.Instance.AIController.MoveTo(keyPoints.LevelMenuEndPoint.transform.position);

            GameStatics.UI.NavButtons.EnableBackButton();
        }

        private IEnumerator ScrollToMainMenuRoutine()
        {
            GameStatics.UI.NavButtons.DisableBackButton();
            GameStatics.Camera.StartFollowing(keyPoints.MainMenuCamPoint.transform, 2.5f);
            yield return new WaitForSeconds(1.1f);

            AnimateMainMenuScene();
        }
    }
}
