using UnityEngine;
using System.Collections;

namespace ClumsyBat.Menu
{
    public class MainMenuTransitions
    {
        private KeyPointsHandler keyPoints;
        private MenuButtonScripts menuButtons;

        private enum States
        {
            MainMenu, DropdownArea, LevelSelect
        }
        private States state;

        public MainMenuTransitions()
        {
            state = States.MainMenu;
            keyPoints = Object.FindObjectOfType<KeyPointsHandler>();
            menuButtons = Object.FindObjectOfType<MenuButtonScripts>();
        }

        public void GotoDropdownArea()
        {
            if (state != States.DropdownArea)
            {
                state = States.DropdownArea;
                GameStatics.Camera.StartFollowing(keyPoints.DropdownAreaPoint.transform, 4.0f);
            }
        }

        public void GotoMainMenuArea()
        {
            if (state == States.DropdownArea)
            {
                GameStatics.GameManager.StartCoroutine(HideDropdownRoutine());
            }
            else
            {
                GameStatics.Camera.StartFollowing(keyPoints.MainMenuCamPoint.transform, 4.0f);
            }
            state = States.MainMenu;
            
            menuButtons.ShowAll();
        }

        public void AnimateMainMenuScene()
        {
            PlayerManager.Instance.PossessByAI();
            PlayerManager.Instance.Clumsy.Physics.Enable();
            PlayerManager.Instance.Clumsy.Physics.EnableGravity();
            GameStatics.GameManager.ResumeGameFromMenu();

            PlayerManager.Instance.SetPlayerPosition(keyPoints.EntryPoint.transform.position);
            PlayerManager.Instance.AIController.MoveTo(keyPoints.EntryLandingPoint.transform);
            menuButtons.ShowAll();
        }

        public void AnimateToLevelSelect()
        { 
            PlayerManager.Instance.PossessByAI();
            PlayerManager.Instance.AIController.MoveTo(keyPoints.MainMenuCamPoint.transform, AnimateThroughCave);
            state = States.LevelSelect;
        }

        public void ShowLevelSelect()
        {
            PlayerManager.Instance.PossessByAI();
            GameStatics.Camera.GotoPointImmediate(keyPoints.LevelMapStart.transform.position.x);
            GameStatics.GameManager.StartCoroutine(AnimateLevelSelect());
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

            GameStatics.GameManager.StartCoroutine(AnimateLevelSelect());
        }

        private IEnumerator AnimateLevelSelect()
        {
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

        private IEnumerator HideDropdownRoutine()
        {
            yield return GameStatics.GameManager.StartCoroutine(GameStatics.UI.DropdownMenu.RaiseMenuRoutine());
            GameStatics.Camera.StartFollowing(keyPoints.MainMenuCamPoint.transform, 4.0f);
        }
    }
}
