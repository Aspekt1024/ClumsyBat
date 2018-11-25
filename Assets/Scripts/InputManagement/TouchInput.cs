using UnityEngine;
using UnityEngine.EventSystems;

namespace ClumsyBat.InputManagement
{
    public class TouchInput
    {
        public enum TouchAction
        {
            None,
            LeftTap,
            RightTap,
            SwipeLeft,
            SwipeRight
        }

        private const float SwipeResistanceX = 70f;
        private const float SwipeResistanceY = 90f;
        private const float StationaryTime = 0.05f; // Time before a tap is registered
        private const float SwipeTime = 0.4f;       // Player must swipe within this time

        private bool isGestureSet;
        private float touchStartPos;
        private float touchStartTime;

        public bool IsJumpHeld { get; private set; }

        private PlayerInputHandler inputHandler;

        public TouchInput(PlayerInputHandler inputHandler)
        {
            this.inputHandler = inputHandler;
        }

        public TouchAction GetAction()
        {
            if (Input.touches.Length == 0) return TouchAction.None;
            Touch touch = Input.touches[0];

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return TouchAction.None;

            if (touch.phase == TouchPhase.Ended)
            {
                IsJumpHeld = false;
            }

            if (touch.phase == TouchPhase.Began)
            {
                isGestureSet = false;
                touchStartPos = touch.position.x;
                touchStartTime = Time.time;
                IsJumpHeld = true;
                return TouchAction.None;
            }
            else if (isGestureSet)
            {
                return TouchAction.None;
            }

            switch (touch.phase)
            {
                case TouchPhase.Stationary:
                    return ResolveStationaryTouch(touch.position.x);
                case TouchPhase.Moved:
                    return ResolveMovedTouch(touch.position.x);
                case TouchPhase.Ended:
                    return ResolveEndedTouch(touch.position.x);
            }
            return TouchAction.None;
        }

        private TouchAction ResolveStationaryTouch(float touchPosX)
        {
            if (IsWithinStationaryTime())
            {
                return TouchAction.None;
            }
            else
            {
                isGestureSet = true;
                if (touchPosX < 0.5 * Screen.width)
                {
                    return TouchAction.LeftTap;
                }
                else
                {
                    return TouchAction.RightTap;
                }
            }
        }

        private TouchAction ResolveMovedTouch(float touchPosX)
        {
            if (IsWithinSwipeTime() && RightSwipeDistanceAchieved(touchPosX))
            {
                isGestureSet = true;
                return TouchAction.SwipeRight;
            }
            else if (IsWithinSwipeTime() && LeftSwipeDistanceAchieved(touchPosX))
            {
                isGestureSet = true;
                return TouchAction.SwipeLeft;
            }
            else
            {
                return ResolveStationaryTouch(touchPosX);
            }
        }

        private TouchAction ResolveEndedTouch(float touchPosX)
        {
            return ResolveMovedTouch(touchPosX);
        }

        private bool IsWithinSwipeTime()
        {
            return Time.time < touchStartTime + SwipeTime;
        }

        private bool IsWithinStationaryTime()
        {
            return Time.time < touchStartTime + StationaryTime;
        }

        private bool LeftSwipeDistanceAchieved(float touchPosX)
        {
            return touchPosX - touchStartPos < -SwipeResistanceX;
        }

        private bool RightSwipeDistanceAchieved(float touchPosX)
        {
            return touchPosX - touchStartPos > SwipeResistanceX;
        }
    }
}