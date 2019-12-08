using ClumsyBat.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClumsyBat
{
    public class LevelGameHandler : MonoBehaviour
    {
        public GameMusicControl GameMusic;
        
        [HideInInspector] public LevelScript Level;
        [HideInInspector] public CaveHandler CaveHandler;

        private Player player;
        private float _resumeTimerStart;
        private const float ResumeTimer = 3f;
        private bool caveExitAutoFlightTriggered;

        private void Start()
        {
            Level = FindObjectOfType<LevelScript>();
            GameMusic = Level.gameObject.AddComponent<GameMusicControl>(); // TODO this ourselves
            CaveHandler = FindObjectOfType<CaveHandler>();
            player = GameStatics.Player.Clumsy;
            
            EventListener.OnDeath += OnDeath;
        }

        private void OnDisable()
        {
            EventListener.OnDeath -= OnDeath;
        }

        public void SetupLevel()
        {
            Level.SetupGame();
        }

        public void StartLevel()
        {
            StartCoroutine(LevelStartRoutine());

            if (GameStatics.LevelManager.IsBossLevel)
            {
                Debug.Log("Starting boss level.");
            }
            else
            {
                SetCameraEndPoint();
                GameStatics.UI.GameHud.SetCurrencyText("0/" + GameStatics.LevelManager.NumMoths);
            }
        }

        public void EndLevelMainPath()
        {
            //TODO pause scores
            StartCoroutine(CaveExitRoutine());
        }

        private void SetCameraEndPoint()
        {
            GameStatics.Camera.SetEndPoint(CaveHandler.GetEndCave().transform.position.x);
        }

        private IEnumerator LevelStartRoutine()
        {
            yield return StartCoroutine(GameStatics.Player.Sequencer.PlaySequence(LevelAnimationSequencer.Sequences.CaveEntrance));

            LevelStart();
            GameStatics.Camera.StartFollowing();
        }

        private IEnumerator CaveExitRoutine()
        {
            yield return StartCoroutine(GameStatics.Player.Sequencer.PlaySequence(LevelAnimationSequencer.Sequences.CaveExit));
            LevelComplete();
        }

        private void LevelStart()
        {
            GameStatics.Player.PossessByPlayer();
            Level.StartGame();
        }

        private void OnDeath()
        {
            GetComponent<AudioSource>().Stop();
        }

        /// <summary>
        /// Used for when a level is completed
        /// </summary>
        /// <param name="viaSecretPath"></param>
        public void LevelComplete(bool viaSecretPath = false)
        {
            Level.stateHandler.SetLevelOver(true);
            var nextLevelData = Level.LevelWon(viaSecretPath);
            
            if (viaSecretPath)
            {
                if (nextLevelData != null)
                {
                    GameStatics.GameManager.LoadLevel(nextLevelData.ID);
                    return;
                }
            }

            var nextLevel = LevelProgressionHandler.GetNextLevel(GameStatics.LevelManager.Level);
            if (nextLevel == LevelProgressionHandler.Levels.Credits)
            {
                StartCoroutine(GameCompleteRoutine());
            }
            else
            {
                GameStatics.UI.DropdownMenu.ShowLevelCompletion(GameStatics.LevelManager.Level, nextLevel);
            }
        }

        public void GameOver()
        {
            Level.stateHandler.SetLevelOver(true);
            Level.ShowGameoverMenu();
        }

        private IEnumerator GameCompleteRoutine()
        {
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(GameStatics.UI.LoadingScreen.ShowLoadScreen(3f));
            SceneManager.LoadScene("Credits");
        }
    }
}
