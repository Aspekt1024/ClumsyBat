
using ClumsyBat.Players;

namespace ClumsyBat.LevelManagement
{
    public class LevelStateHandler
    {
        private float previousPlayerPos;

        public bool GameHasStarted { get; private set; }

        public void Begin()
        {
            GameStatics.Data.GameState.Reset();
            GameHasStarted = true;
        }

        public void Tick(float deltaTime)
        {
            RecordTime(deltaTime);
            RecordDistance(deltaTime);
        }

        private void RecordTime(float deltaTime)
        {
            GameStatics.Data.Stats.TotalTime += deltaTime;

            if (!GameHasStarted || GameStatics.GameManager.IsPaused)
            {
                GameStatics.Data.Stats.IdleTime += deltaTime;
                return;
            }

            GameStatics.Data.Stats.PlayTime += deltaTime;
            GameStatics.Data.GameState.TimeTaken += deltaTime;

            GameStatics.UI.GameHud.UpdateTimer(GameStatics.Data.GameState.TimeTaken);
        }

        private void RecordDistance(float deltaTime)
        {
            Player player = GameStatics.Player.Clumsy;
            if (player.State.IsNormal)
            {
                GameStatics.Data.GameState.Distance += player.model.position.x - previousPlayerPos;
                previousPlayerPos = player.model.position.x;
            }
        }
    }
}
