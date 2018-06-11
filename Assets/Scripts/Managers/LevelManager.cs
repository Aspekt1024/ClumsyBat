using System.Collections;
using UnityEngine;

namespace ClumsyBat.Managers
{
    public class LevelManager : ManagerBase<LevelManager>
    {
        public LevelObjectHandler ObjectHandler;
        public LevelGameHandler GameHandler;

        private int currentLevel;
        private bool loadingLevel;
        
        public static void StartLevel(LevelProgressionHandler.Levels level)
        {
            GameData.Instance.Level = level;
            Instance.StartCoroutine(Instance.LoadLevel(level));
        }
        
        private IEnumerator LoadLevel(LevelProgressionHandler.Levels level)
        {
            Toolbox.Instance.Debug = false;

            loadingLevel = true;
            StartCoroutine(LoadLevelObjects(level));

            yield return ClumsySceneManager.FadeOut();

            while (loadingLevel)
            {
                yield return null;
            }
            
            ClumsySceneManager.SwitchState(ClumsySceneManager.GameState.InLevel);

            yield return ClumsySceneManager.FadeIn();

            GameHandler.StartLevel();
        }

        private IEnumerator LoadLevelObjects(LevelProgressionHandler.Levels level)
        {
            yield return ObjectHandler.LoadLevel(level);
            loadingLevel = false;
        }
    }
}
