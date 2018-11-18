using ClumsyBat.Menu;
using ClumsyBat.Players;
using System;
using System.Collections;
using UnityEngine;

using Levels = LevelProgressionHandler.Levels;

namespace ClumsyBat
{
    /// <summary>
    /// The entry point to Clumsy Bat!
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public enum GameStates
        {
            MainMenu, InLevel
        }
        private GameStates state;

        private GameStatics statics;

        private GameObject levelObject;
        private GameObject menuObject;
        
        private MainMenuTransitions mainMenuTransitions;

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            // All of the core managers are created in GameStatics
            statics = new GameStatics();
            state = GameStates.MainMenu;

            levelObject = GameObject.Find("Level");
            menuObject = GameObject.Find("MainMenu");

            GameStatics.Data.LoadData(GameDataLoaded);
            mainMenuTransitions = new MainMenuTransitions();
        }

        public bool CanReceivePlayerInput { get { return !IsPaused; } } // TODO review this - e.g. in tooltip, in menu, cutscene etc
        public bool AwaitingPlayerInput { get { return false; } }

        public void PauseGame()
        {
            // TODO Play pause sound
            IsPaused = true;
            Time.timeScale = 0;
            GameStatics.Data.SaveData();
        }

        public void ResumeGame()
        {
            // Play resume sound
            IsPaused = true;
            Time.timeScale = 1;
        }

        public void GotoMenuScene()
        {
            if (state == GameStates.MainMenu)
            {
                mainMenuTransitions.AnimateToMainMenu();
            }
            else
            {
                state = GameStates.MainMenu;
                StartCoroutine(SwitchSceneRoutine(FadeToMainMenu));
            }
        }

        public void LoadLevel(Levels level)
        {
            StartCoroutine(LoadLevelRoutine(level));
        }

        private IEnumerator LoadLevelRoutine(Levels level)
        {
            levelObject.SetActive(true);
            GameStatics.LevelManager.LoadLevel(level);
            yield return StartCoroutine(GameStatics.UI.LoadingScreen.ShowLoadScreen());

            menuObject.SetActive(false);
            levelObject.SetActive(true);
            GameStatics.UI.DropdownMenu.Hide();
            GameStatics.Camera.SwitchToLevelCamera();
            state = GameStates.InLevel;

            yield return StartCoroutine(GameStatics.UI.LoadingScreen.HideLoadScreen());
            GameStatics.LevelManager.BeginLevel();
        }

        private IEnumerator SwitchSceneRoutine(Action sceneAction)
        {
            yield return StartCoroutine(GameStatics.UI.LoadingScreen.ShowLoadScreen());
            sceneAction.Invoke();
            StartCoroutine(GameStatics.UI.LoadingScreen.HideLoadScreen());
        }

        private void GameDataLoaded()
        {
            GameStatics.LevelManager.Init();

            FadeToMainMenu();
            StartCoroutine(GameStatics.UI.LoadingScreen.HideLoadScreen());
        }

        private void FadeToMainMenu()
        {
            GameStatics.Camera.SwitchToMenuCamera();
            menuObject.SetActive(true);
            levelObject.SetActive(false);

            mainMenuTransitions.AnimateMainMenuScene();
        }
    }
}
