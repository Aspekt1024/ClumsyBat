using UnityEngine;

namespace ClumsyBat.Players
{
    public class PlayerTriggerHandler : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            switch (collider.tag)
            {
                case "Projectile":
                    GameStatics.Player.Clumsy.TakeDamage(collider.transform, collider.tag, Vector2.zero);
                    break;
                case "Moth":
                    Moth moth = collider.GetComponentInParent<Moth>();
                    moth.ConsumeMoth();
                    break;
                case "Boss":
                    GameStatics.Player.Clumsy.TakeDamage(collider.transform, collider.tag, Vector2.zero);
                    break;
            }

            if (collider.CompareTag("SecretExit"))
            {
                GameStatics.Player.Clumsy.State.SetState(PlayerState.States.InSecretPath, true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("SecretExit") && GameStatics.Player.Clumsy.model.position.y > -6f)
            {
                GameStatics.Player.Clumsy.State.SetState(PlayerState.States.InSecretPath, false);
            }
        }
    }
}
