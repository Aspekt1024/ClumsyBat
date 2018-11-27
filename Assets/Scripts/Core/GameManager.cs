﻿using ClumsyBat.Menu;
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
        public enum StartupModes
        {
            MainMenu, InLevel
        }
        public StartupModes StartupMode = StartupModes.MainMenu;

        public bool DebugMode = false;

        public enum GameStates
        {
            MainMenu, InLevel
        }
        private GameStates state;

        public GameObject LevelObject;
        public GameObject MenuObject;
        public GameObject BossObject;

        private GameStatics statics;
        
        private MainMenuTransitions mainMenuTransitions;

        public bool IsPaused { get; private set; }
        public bool IsInMenu { get { return state == GameStates.MainMenu; } }
        public bool IsInLevel { get { return state == GameStates.InLevel; } }

        private void Awake()
        {
            // All of the core managers are created in GameStatics
            statics = new GameStatics();
            state = GameStates.MainMenu;

            GameStatics.Data.LoadData(GameDataLoaded);
            mainMenuTransitions = new MainMenuTransitions();
        }

        public bool CanReceivePlayerInput { get { return !IsPaused; } } // TODO review this - e.g. in tooltip, in menu, cutscene etc
        public bool AwaitingPlayerInput { get { return false; } }

        public void PauseGame()
        {
            IsPaused = true;
            Time.timeScale = 0;
            GameStatics.Data.SaveData();
        }

        public void ResumeGame()
        {
            StartCoroutine(ResumeGameRoutine());
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
            ResumeGame();
            yield return StartCoroutine(GameStatics.UI.LoadingScreen.ShowLoadScreen());
            MenuObject.SetActive(false);

            GameStatics.LevelManager.LoadLevel(level);

            GameStatics.UI.DropdownMenu.HideImmediate();
            state = GameStates.InLevel;

            yield return StartCoroutine(GameStatics.UI.LoadingScreen.HideLoadScreen());
            GameStatics.LevelManager.BeginLevel();
        }

        private IEnumerator SwitchSceneRoutine(Action sceneAction)
        {
            yield return StartCoroutine(GameStatics.UI.LoadingScreen.ShowLoadScreen());
            GameStatics.UI.DropdownMenu.HideImmediate();
            SetUnpaused();
            sceneAction.Invoke();
            StartCoroutine(GameStatics.UI.LoadingScreen.HideLoadScreen());
        }

        private void GameDataLoaded()
        {
            GameStatics.LevelManager.Init();

            if (StartupMode == StartupModes.MainMenu)
            {
                FadeToMainMenu();
                StartCoroutine(GameStatics.UI.LoadingScreen.HideLoadScreen());
            }
            else
            {
                LoadLevel(Levels.Main1);
            }
        }

        private void FadeToMainMenu()
        {
            var pos = GameStatics.Camera.MenuCamera.transform.position;
            pos.x = 0f;
            GameStatics.Camera.MenuCamera.transform.position = pos;
            MenuObject.SetActive(true);
            LevelObject.SetActive(false);
            BossObject.SetActive(false);
            GameStatics.Camera.SwitchToMenuCamera();

            mainMenuTransitions.AnimateMainMenuScene();
        }

        private IEnumerator ResumeGameRoutine()
        {
            yield return GameStatics.UI.DropdownMenu.RaiseMenuRoutine();
            SetUnpaused();
        }

        private void SetUnpaused()
        {
            IsPaused = false;
            Time.timeScale = 1;
        }
    }
}
