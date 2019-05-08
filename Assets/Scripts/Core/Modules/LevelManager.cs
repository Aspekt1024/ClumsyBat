using ClumsyBat.DataManagement;
using ClumsyBat.LevelManagement;

using Levels = LevelProgressionHandler.Levels;
using LevelCompletionPaths = ClumsyBat.LevelManagement.LevelCompletionHandler.LevelCompletionPaths;
using UnityEngine;
using ClumsyBat.Objects;

namespace ClumsyBat
{
    public class LevelManager
    {
        public LevelGameHandler GameHandler;

        private BossHandler bossHandler;
        private LevelScript levelScript;
        private LevelButtonHandler buttonHandler;

        public Levels Level { get; set; }
        public int NumMoths;
        public int ScoreToBeat;
        
        public LevelManager()
        {
            GameObject scriptsObject = GameObject.Find("LevelScripts");
            if (scriptsObject == null)
            {
                Debug.Log("What are you doing... the LevelScripts object doesn't exist...");
                return;
            }

            levelScript = scriptsObject.GetComponent<LevelScript>();
            GameHandler = scriptsObject.GetComponent<LevelGameHandler>();
            buttonHandler = Object.FindObjectOfType<LevelButtonHandler>();
            bossHandler = levelScript.BossHandler;

            bossHandler.gameObject.SetActive(false);
        }

        public bool IsBossLevel => Level.ToString().Contains("Boss");
        public bool IsInPlayMode => levelScript.stateHandler.GameHasStarted && !levelScript.stateHandler.IsLevelOver;

        private LevelCompletionHandler completionHandler = new LevelCompletionHandler();

        public void EndOfLevelReached()
        {
            GameHandler.EndLevelMainPath();
        }

        /// <summary>
        /// Called by LevelScript.LevelWon
        /// </summary>
        public LevelData LevelCompleted(LevelCompletionPaths path)
        {
            EventListener.LevelCompleted();
            return completionHandler.LevelCompleted(GameStatics.LevelManager.Level, path);
        }

        public bool IsLevelCompleted(Levels level)
        {
            LevelData levelData = GameStatics.Data.LevelDataHandler.GetLevelData(level);
            return levelData.LevelCompleted;
        }

        public void LoadLevel(Levels level)
        {
            SetInitialState();
            Level = level;

            GameStatics.GameManager.LevelObject.SetActive(true);
            GameStatics.GameManager.BossObject.SetActive(IsBossLevel);
            GameStatics.Objects.ClearExistingCave();
            
            if (IsBossLevel)
            {
                bossHandler.LoadBoss();
            }
            else
            {
                TextAsset levelTxt = (TextAsset)Resources.Load("LevelXML/" + level.ToString());
                LevelContainer levelData = LevelContainer.LoadFromText(levelTxt.text);
                GameStatics.Objects.SetupLevel(levelData);

                NumMoths = GetNumMoths(levelData);
                ScoreToBeat = levelData.ScoreToBeat;
            }
        }

        private void SetInitialState()
        {
            Vector2 startPos = new Vector2(-Toolbox.TileSizeX / 2, -0.7f);
            GameStatics.Player.SetPlayerPosition(startPos);
            GameStatics.Objects.ObjectHandler.DisableAllObjects();

            var pos = GameStatics.Camera.LevelCamera.transform.position;
            pos.x = 0f;
            GameStatics.Camera.LevelCamera.transform.position = pos;
            GameStatics.Camera.SwitchToLevelCamera();
            GameStatics.Player.Clumsy.fog.Initialise();
        }

        public void BeginLevel()
        {
            GameHandler.StartLevel();
        }

        public void Init()
        {
            buttonHandler.Init();
        }

        private int GetNumMoths(LevelContainer levelObjects)
        {
            int numMoths = 0;
            foreach (var cave in levelObjects.Caves)
            {
                numMoths += cave.Moths.Length;
            }
            return numMoths;
        }
    }
}
