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
        public float moveSpeed = 5.5f;

        public Transform model;
        public Lantern lantern;
        public FogEffect fog;
        public DeathOverlay deathOverlay;

        public Controller Controller { get; set; }
        public PlayerState State { get; private set; }
        public PlayerPhysicsHandler Physics { get; private set; }
        public ClumsyAbilityHandler Abilities { get; private set; }
        
        private ClumsyAnimator animator;
        private ClumsyAudioControl audioControl;

        public void InitAwake()
        {
            Abilities = new ClumsyAbilityHandler(this);
            Physics = new PlayerPhysicsHandler(this);
            State = new PlayerState(this);

            animator = new ClumsyAnimator(this);
            audioControl = gameObject.AddComponent<ClumsyAudioControl>();
            
            Abilities.SetData(GameStatics.Data.Abilities);
        }

        private void FixedUpdate()
        {
            const float lowerLevelBound = -7f;

            if (!GameStatics.LevelManager.IsInPlayMode) return;
            
            var dist = Time.fixedDeltaTime * Mathf.Abs(Physics.Body.velocity.x);
            GameStatics.Data.Stats.TotalDistance += dist;
            if (State.IsRushing)
            {
                GameStatics.Data.Stats.DashDistance += dist;
            }

            if (model.position.y < lowerLevelBound)
            {
                HandleFallenOffLevel();
            }
        }

        public void SetSpeed(float speed)
        {
            Physics.SetHorizontalVelocity(speed);
        }

        public void DeactivateRush()
        {
            Abilities.CancelAction(DirectionalActions.Dash);
        }

        public void Stun(float duration)
        {
            Debug.Log("not stunned... implement this");
        }

        public void TakeDamage(Transform obj, string otherTag, Vector2 point)
        {
            GameStatics.Data.Stats.DamageTaken++;
            if (State.IsShielded) return;
            
            StartCoroutine(Knockback(point));

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
                    case "Spider":
                        GameStatics.Data.Stats.SpiderDeaths++;
                        break;
                    default:
                        GameStatics.Data.Stats.UnknownDeaths++;
                        break;
                }
                Die(obj);
            }
        }

        public void Animate(ClumsyAnimations anim)
        {
            animator.PlayAnimation(anim);
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
            var scale = model.localScale;
            if (!(scale.x > 0)) return;
            
            model.localScale = new Vector3(-scale.x, scale.y, scale.z);
            model.position += Vector3.right * .5f;

            var rotation = model.localEulerAngles;
            rotation.z = -rotation.z;
            model.localEulerAngles = rotation;

            lantern.GetComponent<HingeJoint2D>().limits = new JointAngleLimits2D()
            {
                min = -180f,
                max = -45f
            };
        }

        public void FaceRight()
        {
            Vector3 scale = model.transform.localScale;
            if (scale.x < 0)
            {
                model.localScale = new Vector3(-scale.x, scale.y, scale.z);
                model.position += Vector3.left * .5f;

                var rotation = model.localEulerAngles;
                rotation.z = -rotation.z;
                model.localEulerAngles = rotation;

                lantern.GetComponent<HingeJoint2D>().limits = new JointAngleLimits2D()
                {
                    min = 45f,
                    max = 180f
                };
            }
        }

        private IEnumerator Knockback(Vector2 contactPoint)
        {
            const float knockbackSpeed = 6f;
            const float knockbackDuration = 0.4f;
            
            var pos = model.position;
            Physics.Body.velocity = -(contactPoint - new Vector2(pos.x, pos.y)).normalized * knockbackSpeed;
            
            State.SetState(PlayerState.States.Knockback, true);
            yield return new WaitForSeconds(knockbackDuration);
            State.SetState(PlayerState.States.Knockback, false);
        }

        public bool IsFacingRight => model.transform.localScale.x > 0;
        

        public void ResetState()
        {
            State.Reset();
            lantern.Reattach();
            Abilities.Shield.SetCharges(1);
            GameStatics.Player.AIController.DisableHover();
        }
        
        private void Die(Transform otherTf)
        {
            if (!State.IsAlive) return;

            DoAction(StaticActions.Unperch);
            State.SetState(PlayerState.States.Alive, false);

            EventListener.Death();
            GameStatics.Data.Stats.Deaths += 1;

            Physics.DisableCollisions();
            lantern.Drop();
            
            GameStatics.Camera.StopFollowing();
            
            StartCoroutine(DeathRoutine(otherTf));
        }

        private IEnumerator DeathRoutine(Transform otherTf)
        {
            var originalSortDetails = SetToFront(otherTf);
            
            model.GetComponent<SpriteRenderer>().sortingLayerName = "UIFront";
            deathOverlay.Show();

            audioControl.PlaySound(PlayerSounds.Collision);    // TODO replace with something... better? like an "ow!"
            GameStatics.GameManager.PauseGame();
            yield return new WaitForSecondsRealtime(1f);
            GameStatics.GameManager.ResumeGame();
            animator.PlayAnimation(ClumsyAnimations.Die);

            Physics.DisableGravity();
            yield return new WaitForSecondsRealtime(0.3f);
            Physics.EnableGravity();
            yield return new WaitForSecondsRealtime(0.5f);

            deathOverlay.Hide();
            model.GetComponent<SpriteRenderer>().sortingLayerName = "Player";

            RevertLayerDetails(otherTf, originalSortDetails);
        }

        private struct RenderDetails
        {
            public int sortOrder;
            public string sortLayer;
        }
        
        private RenderDetails SetToFront(Transform otherTf)
        {
            var originalRenderDetails = new RenderDetails();

            if (otherTf == null) return originalRenderDetails;
            
            var otherRenderer = otherTf.GetComponent<SpriteRenderer>();
            if (otherRenderer == null || !otherRenderer.gameObject.activeSelf)
            {
                otherRenderer = otherTf.GetComponentInParent<SpriteRenderer>();
            }
            if (otherRenderer == null || !otherRenderer.gameObject.activeSelf)
            {
                otherRenderer = otherTf.GetComponentInParent<SpriteRenderer>();
            }

            if (otherRenderer != null)
            {
                originalRenderDetails.sortLayer = otherRenderer.sortingLayerName;
                originalRenderDetails.sortOrder = otherRenderer.sortingOrder;
                otherRenderer.sortingLayerName = "UIFront";
                otherRenderer.sortingOrder = 100;
            }

            return originalRenderDetails;
        }

        private void RevertLayerDetails(Transform otherTf, RenderDetails details)
        {
            if (otherTf == null) return;
            var otherRenderer = otherTf.GetComponent<SpriteRenderer>();
            if (otherRenderer == null) return;
            otherRenderer.sortingLayerName = details.sortLayer;
            otherRenderer.sortingOrder = details.sortOrder;
        }

        private void HandleFallenOffLevel()
        {
            if (!State.IsAlive)
            {
                GameStatics.LevelManager.GameHandler.GameOver();
            }
            else if (State.IsInSecretPath)
            {
                GameStatics.LevelManager.GameHandler.LevelComplete(true);
            }
            else
            {
                Die(null);
            }
        }
    }
}
