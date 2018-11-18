using UnityEngine;
using static LevelProgressionHandler;

namespace ClumsyBat.DataManagement
{
    /// <summary>
    /// Non-persistent game data
    /// </summary>
    public class GameData
    {
        // Game State
        public int CollectedCurrency;
        public float TimeTaken;
        public float Distance;

        public bool IsUntouched;
        public bool OneDamageTaken;
        
        private int mothsEaten;
        public int MothsEaten
        {
            get { return mothsEaten; }
            set
            {
                mothsEaten = value;
                GameStatics.UI.GameHud.SetCurrencyText(mothsEaten + "/" + GameStatics.LevelManager.NumMoths);
            }
        }

        public void Reset()
        {
            CollectedCurrency = 0;
            TimeTaken = 0;
            Distance = 0;
            IsUntouched = true;
            OneDamageTaken = false;
            MothsEaten = 0;
        }

        public float Score
        {
            get
            {
                return ScoreCalculator.GetScore(Distance, mothsEaten, TimeTaken);
            }
        }
    }
}
