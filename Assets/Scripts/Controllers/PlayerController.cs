using ClumsyBat.InputManagement;
using ClumsyBat.Players;
using DirectionalActions = ClumsyBat.Players.ClumsyAbilityHandler.DirectionalActions;
using PlayerActions = ClumsyBat.InputManagement.PlayerInputHandler.PlayerActions;

namespace ClumsyBat.Controllers
{
    public class PlayerController : Controller
    {
        private PlayerInputHandler input;
        private Player player;

        private void Start()
        {
            input = new PlayerInputHandler(this);
            player = FindObjectOfType<Player>();
        }

        private void Update()
        {
            var action = input.GetPlayerAction();
            if (action == PlayerActions.JumpLeft || action == PlayerActions.JumpRight)
            {
                Toolbox.Tooltips.InputReceived();
            }

            if (!CanReceiveInput()) return;

            switch (action)
            {
                case PlayerActions.JumpLeft:
                    player.DoAction(DirectionalActions.Jump, MovementDirections.Left);
                    break;
                case PlayerActions.JumpRight:
                    player.DoAction(DirectionalActions.Jump, MovementDirections.Right);
                    break;
                case PlayerActions.BoostLeft:
                    player.DoAction(DirectionalActions.Dash, MovementDirections.Left);
                    break;
                case PlayerActions.BoostRight:
                    player.DoAction(DirectionalActions.Dash, MovementDirections.Right);
                    break;
            }
        }

        private bool CanReceiveInput()
        {
            if (controlledObject == null) return false;
            if (!player.State.IsAlive) return false;
            if (!GameStatics.StaticsInitiated) return false;

            return !GameStatics.GameManager.IsPaused;
        }
    }
}
