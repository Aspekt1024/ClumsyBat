using UnityEngine;

namespace ClumsyBat.Players
{
    public class FlapComponent
    {
        private readonly Player player;
        private readonly Rigidbody2D playerBody;

        private float horizontalVelocity;
        private const float verticalVelocity = 9f;
        
        private const float moveDuration = 0.7f;
        private float moveTimer;

        public enum MovementModes
        {
            ForwardOnly, LeftAndRight
        }
        public MovementModes MovementMode { get; set; } = MovementModes.ForwardOnly;

        public FlapComponent(Player player)
        {
            this.player = player;
            horizontalVelocity = player.moveSpeed;
            playerBody = player.model.GetComponent<Rigidbody2D>();
        }

        public void MoveLeft()
        {
            Flap(new Vector2(-horizontalVelocity, verticalVelocity));
        }

        public void MoveRight()
        {
            Flap(new Vector2(horizontalVelocity, verticalVelocity));
        }

        private void Flap(Vector2 velocity)
        {
            if (player.State.IsPerched)
            {
                player.DoAction(ClumsyAbilityHandler.StaticActions.Unperch);
                return;
            }

            if (GameStatics.StaticsInitiated)
            {
                GameStatics.Data.Stats.TotalJumps++;
            }

            playerBody.velocity = velocity;
            player.Animate(ClumsyAnimator.ClumsyAnimations.Flap);
            //player.PlaySound(ClumsyAudioControl.PlayerSounds.Flap);
        }


    }
}
