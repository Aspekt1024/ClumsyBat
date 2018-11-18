using ClumsyBat.Controllers;
using ClumsyBat.Players;
using UnityEngine;

namespace ClumsyBat
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public Player Clumsy;
        public PlayerController Controller;
        public PlayerAIController AIController;

        private Controller currentController;
        
        public void PossessByPlayer()
        {
            currentController = Controller;
            Controller.Possess(Clumsy);
        }

        public void PossessByAI()
        {
            currentController = AIController;
            AIController.Possess(Clumsy);
        }

        public void SetPlayerPosition(Vector2 position)
        {
            Clumsy.Model.transform.position = position;
            Clumsy.Lantern.transform.position = position;
        }

        public bool PossessedByAI { get { return currentController.Equals(AIController); } }
        public bool PossessedByPlayer { get { return currentController.Equals(Controller); } }
    }
}
