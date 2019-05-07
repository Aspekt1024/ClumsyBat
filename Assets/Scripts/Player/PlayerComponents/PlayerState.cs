
using System.Collections.Generic;

namespace ClumsyBat.Players
{
    public class PlayerState
    {
        public enum States
        {
            Grounded, Perched, 
            Shielded, Alive,
            Knockback,
            IsRushing,
            InSecretPath
        }

        private readonly Player player;

        // Defines default states
        private readonly Dictionary<States, bool> statesDict = new Dictionary<States, bool>()
        {
            { States.Alive, true },
            { States.Shielded, false },
            { States.Grounded, false },
            { States.Perched, false },
            { States.Knockback, false },
            { States.IsRushing, false },
            { States.InSecretPath, false },
        };

        public PlayerState(Player player)
        {
            this.player = player;
        }
        
        public bool HasState(States state)
        {
            return statesDict[state];
        }

        public void SetState(States state, bool value)
        {
            statesDict[state] = value;
        }

        public bool IsAlive => statesDict[States.Alive];
        public bool IsShielded => statesDict[States.Shielded];
        public bool IsPerched => statesDict[States.Perched];
        public bool IsNormal => !statesDict[States.Knockback];
        public bool IsRushing => statesDict[States.IsRushing];
        public bool IsKnockedBack => statesDict[States.Knockback];
        public bool IsInSecretPath => statesDict[States.InSecretPath];
    }
}
