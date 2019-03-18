using TouchAction = ClumsyBat.InputManagement.TouchInput.TouchAction;
using KeyboardAction = ClumsyBat.InputManagement.KeyboardInput.KeyboardAction;
using ClumsyBat.Controllers;

namespace ClumsyBat.InputManagement
{
    /// <summary>
    /// Takes touch and keyboard inputs and converts them to a player action
    /// </summary>
    public class PlayerInputHandler
    {
        private readonly TouchInput touchInput;
        private readonly KeyboardInput keyboardInput;

        private PlayerController controller;

        public enum PlayerActions
        {
            JumpLeft,
            JumpRight,
            BoostLeft,
            BoostRight,
            None
        }

        public PlayerInputHandler(PlayerController controller)
        {
            this.controller = controller;

            touchInput = new TouchInput(this);
            keyboardInput = new KeyboardInput(this);
        }

        public PlayerActions GetPlayerAction()
        {
            if (keyboardInput.CheckJumpHeld() || touchInput.IsJumpHeld)
            {
                JumpHeld();
            }
            else
            {
                JumpReleased();
            }

            switch (touchInput.GetAction())
            {
                case TouchAction.LeftTap:
                    return PlayerActions.JumpLeft;
                case TouchAction.RightTap:
                    return PlayerActions.JumpRight;
                case TouchAction.SwipeLeft:
                    return PlayerActions.BoostLeft;
                case TouchAction.SwipeRight:
                    return PlayerActions.BoostRight;
            }

            switch (keyboardInput.GetAction())
            {
                case KeyboardAction.JumpRight:
                    return PlayerActions.JumpRight;
                case KeyboardAction.JumpLeft:
                    return PlayerActions.JumpLeft;
                case KeyboardAction.BoostLeft:
                    return PlayerActions.BoostLeft;
                case KeyboardAction.BoostRight:
                    return PlayerActions.BoostRight;
            }
            
            return PlayerActions.None;
        }

        public void JumpHeld()
        {
            if (controller.IsJumpHeld) return;
            controller.IsJumpHeld = true;
        }

        public void JumpReleased()
        {
            if (!controller.IsJumpHeld) return;
            controller.IsJumpHeld = false;
        }
    }
}
