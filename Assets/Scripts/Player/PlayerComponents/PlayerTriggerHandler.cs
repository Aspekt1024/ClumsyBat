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
            GameStatics.LevelManager.GameHandler.TriggerEntered(collider);
            switch (collider.tag)
            {
                case "Projectile":
                    player.TakeDamage(collider.tag);
                    break;
                case "Moth":
                    Moth moth = collider.GetComponentInParent<Moth>();
                    moth.ConsumeMoth();
                    break;
                case "Boss":
                    GameStatics.Player.Clumsy.TakeDamage(collider.tag);
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            GameStatics.LevelManager.GameHandler.TriggerExited(other);
        }
    }
}
