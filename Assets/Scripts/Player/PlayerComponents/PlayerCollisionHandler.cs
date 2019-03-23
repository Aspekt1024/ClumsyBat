using ClumsyBat.Objects;
using UnityEngine;

namespace ClumsyBat.Players
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        private Player player;
        
        private Vector3 lastContactPoint;
        
        private void Start()
        {
            player = FindObjectOfType<Player>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name.Contains("Cave") || collision.gameObject.name.Contains("Entrance") || collision.gameObject.name.Contains("Exit"))
            {
                if (Mathf.Abs(collision.contacts[0].normal.x) > 0.8f) return;

                bool success = player.Abilities.Perch.TryPerch(collision, player.Controller.IsJumpHeld);
                if (!success)
                {
                    player.SetSpeed(0);
                }
            }
            else
            {
                if (collision.contacts.Length > 0)
                {
                    lastContactPoint = collision.contacts[0].point;
                }
                HandleObjectCollision(collision);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (player.State.IsPerched) return;
            if (collision.gameObject.name.Contains("Cave") || collision.gameObject.name.Contains("Entrance") || collision.gameObject.name.Contains("Exit"))
            {
                foreach (ContactPoint2D cp in collision.contacts)
                {
                    if (Mathf.Abs(cp.normal.x) > 0.8f)
                    {
                        continue;
                    }
                    else
                    {
                        bool success = player.Abilities.Perch.TryPerch(collision, player.Controller.IsJumpHeld);
                        if (!success)
                        {
                            player.SetSpeed(0);
                        }
                        return;
                    }
                }
            }
        }
        
        private void HandleObjectCollision(Collision2D collision)
        {
            player.DeactivateRush();

            switch (collision.collider.tag)
            {
                case "Boss":
                    var boss = collision.collider.GetComponent<Boss>();
                    if (boss != null && boss.IsAlive)
                    {
                        player.TakeDamage(collision.collider.transform, collision.collider.tag, collision.GetContact(0).point);
                    }
                    break;
                case "Stalactite":
                    Stalactite stal = collision.collider.GetComponentInParent<Stalactite>();
                    if (stal.Type == SpawnStalAction.StalTypes.Stalactite)
                    {
                        stal.Crack();
                        player.TakeDamage(collision.collider.transform, collision.collider.tag, collision.GetContact(0).point);
                    }
                    break;
            }
        }
    }
}
