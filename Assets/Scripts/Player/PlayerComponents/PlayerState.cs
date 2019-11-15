
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
        private readonly Dictionary<States, bool> statesDict = new Dictionary<States, bool>();

        public PlayerState(Player player)
        {
            this.player = player;
            Reset();
        }
        
        public bool HasState(States state)
        {
            return statesDict[state];
        }

        public void SetState(States state, bool value)
        {
            if (statesDict.ContainsKey(state))
            {
                statesDict[state] = value;
            }
            else
            {
                statesDict.Add(state, value);
            }
        }

        public void Reset()
        {
            statesDict.Clear();
            statesDict.Add(States.Alive, true);
            statesDict.Add(States.Shielded, false);
            statesDict.Add(States.Grounded, false);
            statesDict.Add(States.Perched, false);
            statesDict.Add(States.Knockback, false);
            statesDict.Add(States.IsRushing, false);
            statesDict.Add(States.InSecretPath, false);
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
