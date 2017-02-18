using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private enum SwipeDirection
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
        Tap = 16
    }

    public enum TapDirection
    {
        Left, Right, Center
    }

    private SwipeDirection _direction = SwipeDirection.None;

    private const float SwipeResistanceX = 70f;
    private const float SwipeResistanceY = 90f;
    private float _touchStartPos;
    private float _touchStartTime;
    private const float StationaryTime = 0.05f; // Time before a tap is registered
    private const float SwipeTime = 0.4f;       // Player must swipe within this time

    private bool _bGestureSet;
    private bool _bSwipeSet;
    private bool _bIsTouching;

    private TapDirection _tapDir;

    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                break;
            }
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _bIsTouching = true;
                    _touchStartPos = touch.position.x;
                    _touchStartTime = Time.time;
                    _direction = SwipeDirection.None;
                    _bGestureSet = false;
                    _bSwipeSet = false;
                    break;

                case TouchPhase.Stationary:
                    if (!_bGestureSet)
                    {
                        if (Time.time > _touchStartTime + StationaryTime)
                        {
                            RegisterTap(touch.position.x);
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (!_bSwipeSet && Time.time < _touchStartTime + SwipeTime && touch.position.x - _touchStartPos > SwipeResistanceX)
                    {
                        _bGestureSet = true;
                        _bSwipeSet = true;
                        _direction = SwipeDirection.Right;
                    }
                    else if (!_bGestureSet && Time.time > _touchStartTime + StationaryTime)
                    {
                        RegisterTap(touch.position.x);
                    }
                    break;

                case TouchPhase.Ended:
                    _bIsTouching = false;
                    if (_touchStartTime + SwipeTime < Time.time && touch.position.x - _touchStartPos > SwipeResistanceX)
                    {
                        _bGestureSet = true;
                        _bSwipeSet = true;
                        _direction = SwipeDirection.Right;
                    }
                    else if (!_bGestureSet)
                    {
                        RegisterTap(touch.position.x);
                    }
                    break;
            }
        }

        #region Keyboard emulation
        if (Input.GetKeyDown("w") || Input.GetKeyDown("q") || Input.GetKeyDown("e"))
        {
            _bIsTouching = true;
            _touchStartTime = Time.time;
            _direction = SwipeDirection.None;
            _bGestureSet = false;
            _bSwipeSet = false;
        }
        else if (Input.GetKeyUp("d"))
        {
            _bGestureSet = true;
            _bSwipeSet = true;
            _direction = SwipeDirection.Right;
        }
        else if (Input.GetKey("w"))
        {
            if (Time.time > _touchStartTime + StationaryTime && !_bGestureSet)
                RegisterTap(Screen.width / 2);
        }
        else if (Input.GetKey("q"))
        {
            if (Time.time > _touchStartTime + StationaryTime && !_bGestureSet)
                RegisterTap(0);
        }
        else if (Input.GetKey("e"))
        {
            if (Time.time > _touchStartTime + StationaryTime && !_bGestureSet)
                RegisterTap(Screen.width);
        }
        else if (Input.GetKeyUp("w"))
        {
            _bIsTouching = false;
            if (!_bGestureSet)
                RegisterTap(Screen.width / 2);
        }
        else if (Input.GetKeyUp("q"))
        {
            _bIsTouching = false;
            if (!_bGestureSet)
                RegisterTap(0);
        }
        else if (Input.GetKeyUp("e"))
        {
            _bIsTouching = false;
            if (!_bGestureSet)
                RegisterTap(Screen.width);
        }
        #endregion
    }

    private void RegisterTap(float touchPosX)
    {
        _bGestureSet = true;
        _direction = SwipeDirection.Tap;
        
        if (touchPosX < 0.4 * Screen.width)
        {
            _tapDir = TapDirection.Left;
        }
        else if (touchPosX > 0.6 * Screen.width)
        {
            _tapDir = TapDirection.Right;
        }
        else
        {
            _tapDir = TapDirection.Center;
        }
    }

    public bool SwipeRightRegistered()
    {
        if (_direction == SwipeDirection.Right)
        {
            _direction = SwipeDirection.None;
            return true;
        }
        return false;
    }

    public bool TapRegistered()
    {
        if (_direction == SwipeDirection.Tap)
        {
            _direction = SwipeDirection.None;
            return true;
        }
        return false;
    }

    public bool TouchHeld()
    {
        return _bIsTouching;
    }

    public void ClearInput()
    {
        _direction = SwipeDirection.None;
    }

    public TapDirection GetTapDir()
    {
        return _tapDir;
    }
}