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

        private enum XDirection
        {
            Left, Right
        }
        private XDirection direction;
        
        private enum States
        {
            Idle, Moving, Dashing
        }
        private States state;

        protected override void Awake()
        {
            base.Awake();
            player = FindObjectOfType<Player>();
        }

        private void Start()
        {
            state = States.Idle;
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
            direction = (player.model.transform.position.x < target.position.x) ? XDirection.Right : XDirection.Left;

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
            player.Physics.DisableGravity();
            player.Physics.SetVelocity(0, 0);
            player.Animate(ClumsyAnimator.ClumsyAnimations.Hover);
        }

        public void DisableHover()
        {
            player.Physics.EnableGravity();
            player.Animate(ClumsyAnimator.ClumsyAnimations.FlapSlower);
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
            if (player.model.transform.position.y < target.position.y - 0.3f)
            {
                if (player.model.transform.position.x < target.position.x)
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
            Vector2 direction = target.position - player.model.transform.position;

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
            if ((state == States.Moving)&&
                ((direction == XDirection.Left && player.model.transform.position.x < target.position.x) ||
                (direction == XDirection.Right && player.model.transform.position.x > target.position.x)))
            {
                return true;
            }
            const float PROXIMITY_THRESHOLD = 0.35f;
            return Vector2.Distance(player.model.transform.position, target.position) < PROXIMITY_THRESHOLD;
        }
    }
}
