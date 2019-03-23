using UnityEngine;

namespace ClumsyBat.Players
{
    public class PlayerTriggerHandler : MonoBehaviour
    {
        private Player player;

        public PlayerTriggerHandler(Player player)
        {
            this.player = player;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            switch (collider.tag)
            {
                case "Projectile":
                    player.TakeDamage(collider.transform, collider.tag, Vector2.zero);
                    break;
                case "Moth":
                    Moth moth = collider.GetComponentInParent<Moth>();
                    moth.ConsumeMoth();
                    break;
                case "Boss":
                    GameStatics.Player.Clumsy.TakeDamage(collider.transform, collider.tag, Vector2.zero);
                    break;
            }
        }
    }
}
