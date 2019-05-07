using UnityEngine;

namespace ClumsyBat.Players
{
    public class ClumsyAbilityHandler
    {
        public enum DirectionalActions
        {
            Jump, Dash
        }

        public enum StaticActions
        {
            Hypersonic, ForcedHypersonic, Unperch, Shield
        }

        public Shield Shield { get; private set; }
        public PerchComponent Perch { get; private set; }
        public FlapComponent Flap { get; private set; }

        private Player player;
        private Hypersonic _hypersonic;
        private RushAbility _rush;

        public ClumsyAbilityHandler(Player player)
        {
            this.player = player;
            SetupAbilities();
        }

        private void SetupAbilities()
        {
            GameObject abilityScripts = new GameObject("Abilities");
            abilityScripts.transform.SetParent(player.transform);

            Shield = abilityScripts.AddComponent<Shield>();

            _rush = abilityScripts.AddComponent<RushAbility>();
            _hypersonic = Object.FindObjectOfType<Hypersonic>();
            Perch = abilityScripts.AddComponent<PerchComponent>();

            Flap = new FlapComponent(player);

            _rush.Setup(player);
            _hypersonic.Setup(player.lantern);
            Shield.Setup(player);
        }

        public void SetData(AbilityControl abilityData)
        {
            _hypersonic.SetData(abilityData.GetHypersonicStats());
            Shield.SetStats(abilityData.GetShieldStats());
            _rush.SetStats(abilityData.GetDashStats());
        }

        public bool DoAction(DirectionalActions action, MovementDirections direction)
        {
            switch (action)
            {
                case DirectionalActions.Jump:
                    return Jump(direction);
                case DirectionalActions.Dash:
                    return _rush.Activate(direction);
                default:
                    return false;
            }
        }

        public bool DoAction(StaticActions action)
        {
            switch (action)
            {
                case StaticActions.Hypersonic:
                    return _hypersonic.ActivateHypersonic();
                case StaticActions.ForcedHypersonic:
                    return _hypersonic.ForceHypersonic();
                case StaticActions.Unperch:
                    return Perch.Unperch();
                case StaticActions.Shield:
                    return Shield.Activate();
                default:
                    return false;
            }
        }

        public void CancelAction(DirectionalActions action)
        {
            switch (action)
            {
                case DirectionalActions.Jump:
                    break;
                case DirectionalActions.Dash:
                    _rush.Deactivate();
                    break;
                default:
                    break;
            }
        }

        public void CancelAction(StaticActions action)
        {
            switch (action)
            {
                case StaticActions.Hypersonic:
                    break;
                case StaticActions.ForcedHypersonic:
                    break;
                case StaticActions.Unperch:
                    break;
                case StaticActions.Shield:
                    break;
                default:
                    break;
            }
        }

        private bool Jump(MovementDirections direction)
        {
            if (direction == MovementDirections.Left)
            {
                player.FaceLeft();
                Flap.MoveLeft();
            }
            else
            {
                player.FaceRight();
                Flap.MoveRight();
            }
            return true;
        }
    }
}
