using System.Collections.Generic;
using UnityEngine;

namespace ClumsyBat.InputManagement
{
    public class KeyboardInput
    {
        public enum KeyboardAction
        {
            JumpRight, JumpLeft,
            BoostLeft, BoostRight,
            None
        }

        private PlayerInputHandler inputHandler;

        public KeyboardInput(PlayerInputHandler inputHandler)
        {
            this.inputHandler = inputHandler;
        }

        private readonly Dictionary<KeyCode, KeyboardAction> keyDownDict = new Dictionary<KeyCode, KeyboardAction>()
        {
            { KeyCode.W, KeyboardAction.JumpRight },
            { KeyCode.E, KeyboardAction.JumpRight },
            { KeyCode.Q, KeyboardAction.JumpLeft },
            { KeyCode.D, KeyboardAction.BoostRight },
            { KeyCode.A, KeyboardAction.BoostLeft }
        };

        private readonly KeyCode[] jumpKeys = new KeyCode[]
        {
            KeyCode.Q, KeyCode.W, KeyCode.E
        };

        public KeyboardAction GetAction()
        {
            foreach (var item in keyDownDict)
            {
                if (Input.GetKeyDown(item.Key))
                {
                    return item.Value;
                }
            }
            return KeyboardAction.None;
        }

        public bool CheckJumpHeld()
        {
            foreach (var key in jumpKeys)
            {
                if (Input.GetKey(key))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
