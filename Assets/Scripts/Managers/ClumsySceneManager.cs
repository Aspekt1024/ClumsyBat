using System;
using System.Collections;
using UnityEngine;

namespace ClumsyBat.Managers
{
    public class ClumsySceneManager : ManagerBase<ClumsySceneManager>
    {
        public GameObject MainMenuObject;
        public GameObject LevelObject;

        public LoadScreen LoadScreen;

        public enum GameState
        {
            MainMenu, InLevel
        }
        private GameState state;
        
        private Action callback;

        public static void SwitchState(GameState state, Action callback)
        {
            Instance.state = state;
            Instance.callback = callback;

            Instance.StartCoroutine(Instance.TransitionToNewState());
        }

        private IEnumerator TransitionToNewState()
        {
            yield return StartCoroutine(LoadScreen.FadeIn());
            
            switch (state)
            {
                case GameState.MainMenu:
                    CameraManager.SwitchToMenuCamera();
                    MainMenuObject.SetActive(true);
                    LevelObject.SetActive(false);
                    break;
                case GameState.InLevel:
                    CameraManager.SwitchToLevelCamera();
                    MainMenuObject.SetActive(false);
                    LevelObject.SetActive(true);
                    break;
                default:
                    break;
            }
            
            yield return StartCoroutine(LoadScreen.FadeOut());
        }


    }
}
