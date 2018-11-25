using ClumsyBat.Players;
using System.Collections;
using UnityEngine;

namespace ClumsyBat
{
    public class GameHandler : MonoBehaviour
    {
        public GameMusicControl GameMusic;

        public enum GameStates
        {
            Starting,
            Normal,
            Paused,
            Resuming,
            PausedForTooltip
        }
        public GameStates GameState { get; private set; }

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

        public void StartLevel()
        {
            StartCoroutine(LevelStartRoutine());
            GameMusic.PlaySound(GameMusicControl.GameTrack.Twinkly);
            SetCameraEndPoint();
            GameStatics.UI.GameHud.SetCurrencyText("0/" + GameStatics.LevelManager.NumMoths);
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

        public void LevelComplete(bool viaSecretPath = false)
        {
            Level.LevelWon(viaSecretPath);
            LevelProgressionHandler.Levels nextLevel = LevelProgressionHandler.GetNextLevel(GameStatics.LevelManager.Level);
            GameStatics.UI.DropdownMenu.ShowLevelCompletion(GameStatics.LevelManager.Level, nextLevel);
        }

        public void GameOver()
        {
            Level.ShowGameoverMenu();
        }
    }
}
