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

        [HideInInspector]
        public LevelAnimationSequencer Sequencer;

        private Controller currentController;

        private void Awake()
        {
            Sequencer = new LevelAnimationSequencer(Clumsy);
        }

        private void FixedUpdate()
        {
            Sequencer.Update(Time.fixedDeltaTime);
        }

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
            Clumsy.Lantern.transform.position = position - new Vector2(0.3f, 1f);
            Clumsy.Lantern.transform.localEulerAngles = new Vector3(0f, 0f, -40f);
        }

        public bool PossessedByAI { get { return currentController.Equals(AIController); } }
        public bool PossessedByPlayer { get { return currentController.Equals(Controller); } }
    }
}
