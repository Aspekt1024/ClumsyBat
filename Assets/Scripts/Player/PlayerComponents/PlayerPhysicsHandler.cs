using UnityEngine;

namespace ClumsyBat.Players
{
    public class PlayerPhysicsHandler
    {
        private const float NORMAL_PLAYER_SPEED = 5.5f;
        private const float NORMAL_GRAVITY_SCALE = 3f;

        public float Speed { get; set; }

        public Rigidbody2D Body { get; private set; }

        private Player player;
        private Collider2D collider;

        public PlayerPhysicsHandler(Player player)
        {
            this.player = player;
            Body = player.model.GetComponent<Rigidbody2D>();
            collider = player.model.GetComponent<Collider2D>();
        }

        public void Tick(float deltaTime)
        {
            if (!player.State.IsShielded)
            {
                float dist = Speed * deltaTime;
                //Body.position += Vector2.right * dist;
            }
        }
        
        public void Disable()
        {
            Body.isKinematic = true;
            Body.constraints = RigidbodyConstraints2D.FreezeRotation;
            DisableCollisions();
        }

        public void Enable()
        {
            collider.enabled = true;
            Body.isKinematic = false;
            Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public void DisableGravity()
        {
            Body.gravityScale = 0f;
        }

        public void EnableGravity()
        {
            Body.gravityScale = NORMAL_GRAVITY_SCALE;
        }

        public void SetVelocity(float xVelocity, float yVelocity)
        {
            Body.velocity = new Vector2(xVelocity, yVelocity);
        }

        public void SetHorizontalVelocity(float velocity)
        {
            Body.velocity = new Vector2(velocity, Body.velocity.y);
        }

        public void SetVerticalVelocity(float velocity)
        {
            Body.velocity = new Vector2(Body.velocity.x, velocity);
        }

        public void SetNormalSpeed()
        {
            SetHorizontalVelocity(NORMAL_PLAYER_SPEED * (player.IsFacingRight ? 1 : -1));
        }

        public void DisableCollisions()
        {
            collider.enabled = false;
        }
    }
}
