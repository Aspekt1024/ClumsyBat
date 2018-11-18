using ClumsyBat.Players;
using System;
using UnityEngine;

namespace ClumsyBat.Controllers
{
    public class PlayerAIController : Controller
    {
        private Player player;

        private Action targetReachedCallback;
        private Vector2 targetPosition;
        private Transform target;          // The target to move to

        private Transform positionMarker;  // Used when not following an object, just moving to a position
        
        private enum States
        {
            Idle, Moving, Dashing
        }
        private States state;

        private void Start()
        {
            player = FindObjectOfType<Player>();
            positionMarker = new GameObject("Player AI Marker").transform;
        }

        public void MoveTo(Vector2 position, Action targetReachedCallback = null)
        {
            positionMarker.position = position;
            MoveTo(positionMarker, targetReachedCallback);
        }

        public void MoveTo(Transform target, Action targetReachedCallback = null)
        {
            this.target = target;
            this.targetReachedCallback = targetReachedCallback;
            state = States.Moving;

            if (player.State.IsPerched) player.DoAction(ClumsyAbilityHandler.StaticActions.Unperch);
            player.Animate(ClumsyAnimator.ClumsyAnimations.Flap);
        }

        public void Dash(MovementDirections direction = MovementDirections.Right)
        {
            player.DoAction(ClumsyAbilityHandler.DirectionalActions.Dash, direction);
        }

        public void DashTo(Transform target, Action targetReachedCallback = null)
        {
            this.target = target;
            this.targetReachedCallback = targetReachedCallback;
            state = States.Dashing;
            player.Physics.DisableGravity();
            player.Animate(ClumsyAnimator.ClumsyAnimations.RushContinuous);
        }

        public void Hover()
        {
            Debug.Log("implement hovering");
        }

        public void DisableHover()
        {
            Debug.Log("disable hover... is this even required?");
        }

        private void Update()
        {
            switch (state)
            {
                case States.Idle:
                    break;
                case States.Moving:
                    MoveTowardsTarget();
                    break;
                case States.Dashing:
                    DashTowardsTarget();
                    break;
                default:
                    break;
            }
        }

        private void MoveTowardsTarget()
        {
            if (player.Model.transform.position.y < target.position.y - 0.3f)
            {
                if (player.Model.transform.position.x < target.position.x)
                {
                    player.DoAction(ClumsyAbilityHandler.DirectionalActions.Jump, MovementDirections.Right);
                }
                else
                {
                    player.DoAction(ClumsyAbilityHandler.DirectionalActions.Jump, MovementDirections.Left);
                }
            }

            if (IsTargetReached())
            {
                TargetReached();
            }
        }

        private void DashTowardsTarget()
        {
            // Setting this up artificially as this is a cinematic thing only
            const float DASH_SPEED = 18f;
            Vector2 direction = target.position - player.Model.transform.position;

            player.Physics.Body.velocity = direction.normalized * DASH_SPEED;

            if (IsTargetReached())
            {
                player.Physics.EnableGravity();
                player.Animate(ClumsyAnimator.ClumsyAnimations.FlapSlower);
                TargetReached();
            }
        }

        private void TargetReached()
        {
            state = States.Idle;
            targetReachedCallback?.Invoke();
        }

        private bool IsTargetReached()
        {
            const float PROXIMITY_THRESHOLD = 0.35f;
            return Vector2.Distance(player.Model.transform.position, target.position) < PROXIMITY_THRESHOLD;
        }
    }
}
