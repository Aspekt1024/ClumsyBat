using ClumsyBat.Menu;
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
        public Levels StartupLevel = Levels.Main1;

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
        public bool IsInMenu => state == GameStates.MainMenu;
        public bool IsInLevel => state == GameStates.InLevel;

        private void Awake()
        {
            // All of the core managers are created in GameStatics
            statics = new GameStatics();
            state = GameStates.MainMenu;

            GameStatics.Data.InitAwake();
            GameStatics.Player.InitAwake();
            
            mainMenuTransitions = new MainMenuTransitions();
        }

        private void Start()
        {
            StartCoroutine(StartGame());
        }
        
        private void Update()
        {
            if (IsInMenu)
            {
                GameStatics.Data.Stats.IdleTime += Time.deltaTime;
            }
        }

        public void PauseGame()
        {
            IsPaused = true;
            Time.timeScale = 0;
            GameStatics.Data.SaveData();
        }

        public void ResumeGameFromMenu()
        {
            StartCoroutine(ResumeGameRoutine());
        }

        public void ResumeGame()
        {
            SetUnpaused();
            GameStatics.Data.GameState.IsPausedForTooltip = false;
        }

        public void GotoMenuScene()
        {
            GameStatics.Player.Clumsy.ResetState();
            if (state == GameStates.MainMenu)
            {
                mainMenuTransitions.AnimateToMainMenu();
            }
            else
            {
                state = GameStates.MainMenu;
                StartCoroutine(SwitchSceneRoutine(FadeToMainMenu));
                GameStatics.Audio.Music.Stop();
            }
        }

        public void GotoLevelSelect()
        {
            state = GameStates.MainMenu;
            StartCoroutine(SwitchSceneRoutine(FadeToLevelSelect));
            GameStatics.Audio.Music.Stop();
        }

        public void LoadLevel(Levels level)
        {
            StartCoroutine(LoadLevelRoutine(level));
        }

        private IEnumerator LoadLevelRoutine(Levels level)
        {
            ResumeGameFromMenu();
            GameStatics.LevelManager.SetupLevel();
            yield return StartCoroutine(GameStatics.UI.LoadingScreen.ShowLoadScreen());
            MenuObject.SetActive(false);

            GameStatics.LevelManager.LoadLevel(level);

            GameStatics.UI.DropdownMenu.HideImmediate();
            state = GameStates.InLevel;

            GameStatics.UI.GameHud.SetLevelText(level);
            Toolbox.Instance.ShowLevelTooltips = (!GameStatics.Data.LevelDataHandler.IsCompleted(level));
            GameStatics.LevelManager.BeginLevel();
            StartCoroutine(GameStatics.UI.LoadingScreen.HideLoadScreen());
        }

        private IEnumerator SwitchSceneRoutine(Action sceneAction)
        {
            yield return StartCoroutine(GameStatics.UI.LoadingScreen.ShowLoadScreen());
            GameStatics.UI.DropdownMenu.HideImmediate();
            SetUnpaused();
            sceneAction.Invoke();
            StartCoroutine(GameStatics.UI.LoadingScreen.HideLoadScreen());
        }

        private IEnumerator StartGame()
        {
            yield return new WaitForSeconds(0.2f);

            GameStatics.LevelManager.Init();

            if (StartupMode == StartupModes.MainMenu && StartupLevel != Levels.Unassigned)
            {
                FadeToMainMenu();
                StartCoroutine(GameStatics.UI.LoadingScreen.HideLoadScreen());
            }
            else
            {
                LoadLevel(StartupLevel);
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

            mainMenuTransitions.GotoMainMenuArea();
            mainMenuTransitions.AnimateMainMenuScene();
        }

        private void FadeToLevelSelect()
        {
            MenuObject.SetActive(true);
            LevelObject.SetActive(false);
            BossObject.SetActive(false);
            GameStatics.Camera.SwitchToMenuCamera();

            mainMenuTransitions.ShowLevelSelect();
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
