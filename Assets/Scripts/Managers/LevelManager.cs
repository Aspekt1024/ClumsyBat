using System.Collections;
using UnityEngine;

namespace ClumsyBat.Managers
{
    public class LevelManager : MonoBehaviour
    {
        private int currentLevel;

        private static LevelManager _instance;

        public static LevelManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("Level Manager does not exist in the scene, but something is tring to access it.");
                }
                return _instance;
            }
        }
        
        public void Awake()
        {
            if (_instance == null)
            {
                Debug.Log("Level manager ready to go!");
                _instance = this;
            }
            else
            {
                Debug.LogWarning("Multiple LevelMangers found in the scene. Removing duplicate");
                Destroy(this);
            }
        }

        public static void StartLevel(LevelProgressionHandler.Levels level)
        {
            GameData.Instance.Level = level;
            Instance.StartCoroutine(Instance.LoadLevel(level));
        }
        
        private IEnumerator LoadLevel(LevelProgressionHandler.Levels levelId)
        {
            GameData.Instance.Level = levelId;
            Toolbox.Instance.Debug = false;
            
            ClumsySceneManager.SwitchState(ClumsySceneManager.GameState.InLevel, LevelLoaded);

            yield return null;
            
            //AsyncOperation levelLoader;

            //yield return StartCoroutine(LoadingOverlay.GetComponent<LoadScreen>().FadeIn());

            //if (levelId.ToString().Contains("Boss"))
            //    levelLoader = SceneManager.LoadSceneAsync("Boss");
            //else
            //levelLoader = SceneManager.LoadSceneAsync("Levels");

        }

        private void LevelLoaded()
        {
        }
    }
}
