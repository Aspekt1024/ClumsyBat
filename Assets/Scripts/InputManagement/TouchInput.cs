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
        
        private bool gestureIsSet;
        private float touchStartPos;
        private float touchStartTime;

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

            if (touch.phase == TouchPhase.Began)
            {
                gestureIsSet = false;
                touchStartPos = touch.position.x;
                touchStartTime = Time.time;
                inputHandler.JumpHeld();
                return TouchAction.None;
            }
            else if (gestureIsSet)
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
                    inputHandler.JumpReleased();
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
                gestureIsSet = true;
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
                gestureIsSet = true;
                return TouchAction.SwipeRight;
            }
            else if (IsWithinSwipeTime() && LeftSwipeDistanceAchieved(touchPosX))
            {
                gestureIsSet = true;
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