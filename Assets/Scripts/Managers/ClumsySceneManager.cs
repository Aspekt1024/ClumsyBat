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

        public static IEnumerator FadeOut()
        {
            yield return Instance.StartCoroutine(Instance.LoadScreen.FadeIn());
        }

        public static IEnumerator FadeIn()
        {
            yield return Instance.StartCoroutine(Instance.LoadScreen.FadeOut());
        }

        public static void SwitchState(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    CameraManager.SwitchToMenuCamera();
                    Instance.MainMenuObject.SetActive(true);
                    Instance.LevelObject.SetActive(false);
                    break;
                case GameState.InLevel:
                    CameraManager.SwitchToLevelCamera();
                    Instance.MainMenuObject.SetActive(false);
                    Instance.LevelObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}
