using ClumsyBat.Controllers;
using System.Collections;
using UnityEngine;

using PlayerSounds = ClumsyAudioControl.PlayerSounds;
using ClumsyAnimations = ClumsyBat.Players.ClumsyAnimator.ClumsyAnimations;
using StaticActions = ClumsyBat.Players.ClumsyAbilityHandler.StaticActions;
using DirectionalActions = ClumsyBat.Players.ClumsyAbilityHandler.DirectionalActions;
using ClumsyBat.UI;

namespace ClumsyBat.Players
{
    [RequireComponent(typeof(PlayerSensor))]
    public class Player : MonoBehaviour, IControllable
    {
        private const float KNOCKBACK_DURATION = 0.55f;

        public float MoveSpeed = 4f;

        public Transform Model;
        public Hypersonic Hypersonic;
        public Lantern Lantern;
        public FogEffect Fog;
        public DeathOverlay DeathOverlay;

        public Controller Controller { get; set; }
        public PlayerState State { get; private set; }
        public PlayerPhysicsHandler Physics { get; private set; }
        public ClumsyAbilityHandler Abilities { get; private set; }
        
        private ClumsyAnimator animator;
        private ClumsyAudioControl audioControl;
        private PlayerSensor sensor;
        
        private void Awake()
        {
            Abilities = new ClumsyAbilityHandler(this);
            Physics = new PlayerPhysicsHandler(this);
            State = new PlayerState(this);

            animator = new ClumsyAnimator(this);
            sensor = GetComponent<PlayerSensor>();
            audioControl = gameObject.AddComponent<ClumsyAudioControl>();
        }

        private void FixedUpdate()
        {
            Physics.Tick(Time.fixedDeltaTime);
        }

        public void SetSpeed(float speed)
        {
            Physics.Speed = speed;
        }

        public void DeactivateRush()
        {
            // TODO this
            Debug.Log("deactivate rush to be implemented");
        }

        public void Stun(float duration)
        {
            Debug.Log("not stunned... implement this");
        }

        public void TakeDamage(Transform obj, string otherTag, Vector2 point)
        {
            if (State.IsShielded) return;

            bool successfullyShielded = DoAction(StaticActions.Shield);

            if (Abilities.Shield.IsAvailable())
            {
                DoAction(StaticActions.Unperch);
                DoAction(StaticActions.Shield);

                GameStatics.Data.GameState.NumTimesTakenDamage++;
            }
            else
            {
                switch (otherTag)
                {
                    case "Stalactite":
                        GameStatics.Data.Stats.ToothDeaths++;
                        break;
                    case "Boss":
                        GameStatics.Data.Stats.BossDeaths++;
                        break;
                    default:
                        GameStatics.Data.Stats.UnknownDeaths++;
                        break;
                }
                Die(obj);
            }
        }

        public void Animate(ClumsyAnimations animation)
        {
            animator.PlayAnimation(animation);
        }

        public bool DoAction(StaticActions action)
        {
            return Abilities.DoAction(action);
        }

        public bool DoAction(DirectionalActions action, MovementDirections direction)
        {
            return Abilities.DoAction(action, direction);
        }

        public void FaceLeft()
        {
            Vector3 scale = Model.transform.localScale;
            if (scale.x > 0)
            {
                Model.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                Model.transform.position += Vector3.right * .5f;
                Lantern.GetComponent<HingeJoint2D>().limits = new JointAngleLimits2D()
                {
                    min = -260f,
                    max = -220f
                };
            }
        }

        public void FaceRight()
        {
            Vector3 scale = Model.transform.localScale;
            if (scale.x < 0)
            {
                Model.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                Model.transform.position += Vector3.left * .5f;

                var rotation = Model.transform.localEulerAngles;
                rotation.z = -rotation.z;
                Model.transform.localEulerAngles = rotation;

                Lantern.GetComponent<HingeJoint2D>().limits = new JointAngleLimits2D()
                {
                    min = -20f,
                    max = 40f
                };
            }
        }

        public IEnumerator Knockback(Vector2 contactPoint)
        {
            float knockbackTimer = 0f;

            float directionModifier = contactPoint.x < transform.position.x ? -1 : 1;

            while (knockbackTimer < KNOCKBACK_DURATION)
            {
                Physics.SetHorizontalVelocity(Mathf.Lerp(directionModifier * 7f, 0f, knockbackTimer / KNOCKBACK_DURATION));
                yield return null;
            }
        }

        public void SetColor(Color color)
        {
            // TODO this
        }

        public bool IsFacingRight { get { return Model.transform.localScale.x > 0; } }

        
        private void Die(Transform otherTf)
        {
            if (!State.IsAlive) return;

            DoAction(StaticActions.Unperch);
            State.SetState(PlayerState.States.Alive, false);

            EventListener.Death();
            GameStatics.Data.Stats.Deaths += 1;

            Physics.DisableCollisions();
            Lantern.Drop();
            
            StartCoroutine(PauseForDeath(otherTf));
        }

        private IEnumerator PauseForDeath(Transform otherTf)
        {
            var otherRenderer = otherTf.GetComponent<SpriteRenderer>();
            if (otherRenderer == null || !otherRenderer.gameObject.activeSelf)
            {
                otherRenderer = otherTf.GetComponentInParent<SpriteRenderer>();
            }
            if (otherRenderer == null || !otherRenderer.gameObject.activeSelf)
            {
                otherRenderer = otherTf.GetComponentInParent<SpriteRenderer>();
            }

            int originalSortOrder = 0;
            string originalSortLayer = otherRenderer.sortingLayerName;
            if (otherRenderer != null)
            {
                originalSortOrder = otherRenderer.sortingOrder;
                otherRenderer.sortingLayerName = "UIFront";
                otherRenderer.sortingOrder = 100;
            }

            Model.GetComponent<SpriteRenderer>().sortingLayerName = "UIFront";
            DeathOverlay.Show();

            audioControl.PlaySound(PlayerSounds.Collision);    // TODO replace with something... better? like an "ow!"
            GameStatics.GameManager.PauseGame();
            yield return new WaitForSecondsRealtime(1f);
            GameStatics.GameManager.ResumeGame();
            animator.PlayAnimation(ClumsyAnimations.Die);

            Physics.DisableGravity();
            yield return new WaitForSecondsRealtime(0.3f);
            Physics.EnableGravity();
            yield return new WaitForSecondsRealtime(0.5f);

            DeathOverlay.Hide();
            Model.GetComponent<SpriteRenderer>().sortingLayerName = "Player";

            if (otherRenderer != null)
            {
                otherRenderer.sortingLayerName = originalSortLayer;
                otherRenderer.sortingOrder = originalSortOrder;
            }
        }
    }
}
