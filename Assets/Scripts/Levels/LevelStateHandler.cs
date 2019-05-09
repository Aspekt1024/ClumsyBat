
using ClumsyBat.Players;

namespace ClumsyBat.LevelManagement
{
    public class LevelStateHandler
    {
        private float previousPlayerPos;

        public bool GameHasStarted { get; private set; }
        public bool IsLevelOver { get; private set; }

        public void Setup()
        {
            GameHasStarted = false; // Game starts after startup phase (in Begin())
            IsLevelOver = false;
            GameStatics.Data.GameState.Reset();
        }
        
        public void Begin()
        {
            GameHasStarted = true;
            IsLevelOver = false;
            previousPlayerPos = GameStatics.Player.Clumsy.model.position.x;
        }

        public void SetLevelOver(bool value)
        {
            GameHasStarted = !value;
            IsLevelOver = value;
        }

        public void Tick(float deltaTime)
        {
            RecordTime(deltaTime);
            RecordDistance(deltaTime);
        }

        private void RecordTime(float deltaTime)
        {
            GameStatics.Data.Stats.TotalTime += deltaTime;

            if (GameStatics.GameManager.IsPaused)
            {
                GameStatics.Data.Stats.IdleTime += deltaTime;
                return;
            }

            if (GameStatics.LevelManager.IsInPlayMode)
            {
                GameStatics.Data.Stats.PlayTime += deltaTime;
                GameStatics.Data.GameState.TimeTaken += deltaTime;
            }
            else
            {
                GameStatics.Data.Stats.IdleTime += deltaTime;
            }

            GameStatics.UI.GameHud.UpdateTimer(GameStatics.Data.GameState.TimeTaken);
        }

        private void RecordDistance(float deltaTime)
        {
            var player = GameStatics.Player.Clumsy;
            if (GameStatics.GameManager.IsPaused || !GameStatics.LevelManager.IsInPlayMode) return;
            if (!player.State.IsNormal) return;
            
            GameStatics.Data.GameState.Distance += player.model.position.x - previousPlayerPos;
            previousPlayerPos = player.model.position.x;
        }
    }
}
