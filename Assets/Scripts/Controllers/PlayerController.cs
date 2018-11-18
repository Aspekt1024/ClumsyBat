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
            if (!CanReceiveInput()) return;

            var action = input.GetPlayerAction();

            if (AwaitingPlayerInput())
            {
                if (action == PlayerActions.JumpLeft || action == PlayerActions.JumpRight)
                {
                    // TODO event handler - got input
                }
            }
            else if (controlledObject != null)
            {
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
        }

        private bool CanReceiveInput()
        {
            if (!player.State.IsAlive) return false;
            if (!GameStatics.StaticsInitiated) return true;

            return GameStatics.GameManager.CanReceivePlayerInput;
        }

        private bool AwaitingPlayerInput()
        {
            if (!GameStatics.StaticsInitiated) return false;
            return GameStatics.GameManager.AwaitingPlayerInput;
        }
    }
}
