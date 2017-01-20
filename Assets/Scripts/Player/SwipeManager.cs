using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public enum SwipeDirection
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
    Tap = 16
}

public class SwipeManager : MonoBehaviour
{
    private SwipeDirection Direction = SwipeDirection.None;

    private float SwipeResistanceX = 70f;
    private float TouchStartPos;
    private float TouchStartTime;
    private const float StationaryTime = 0.06f;
    private const float SwipeTime = 0.4f;

    private bool GestureSet = false;
    private bool SwipeSet = false;

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
                    TouchStartPos = touch.position.x;
                    TouchStartTime = Time.time;
                    Direction = SwipeDirection.None;
                    GestureSet = false;
                    SwipeSet = false;
                    break;

                case TouchPhase.Stationary:
                    if (!GestureSet)
                    {
                        if (Time.time > TouchStartTime + StationaryTime)
                        {
                            GestureSet = true;
                            Direction = SwipeDirection.Tap;
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (!SwipeSet && Time.time < TouchStartTime + SwipeTime && touch.position.x - TouchStartPos > SwipeResistanceX)
                    {
                        GestureSet = true;
                        SwipeSet = true;
                        Direction = SwipeDirection.Right;
                    }
                    else if (!GestureSet && Time.time > TouchStartTime + StationaryTime)
                    {
                        GestureSet = true;
                        Direction = SwipeDirection.Tap;
                    }
                    break;

                case TouchPhase.Ended:
                    if (TouchStartTime + SwipeTime < Time.time && touch.position.x - TouchStartPos > SwipeResistanceX)
                    {
                        GestureSet = true;
                        SwipeSet = true;
                        Direction = SwipeDirection.Right;
                    }
                    else if (!GestureSet)
                    {
                        GestureSet = true;
                        Direction = SwipeDirection.Tap;
                    }
                    break;
            }
        }
    }

    public bool SwipeRegistered()
    {
        if (Direction == SwipeDirection.Right)
        {
            Direction = SwipeDirection.None;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TapRegistered()
    {
        if (Direction == SwipeDirection.Tap)
        {
            Direction = SwipeDirection.None;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ClearInput()
    {
        Direction = SwipeDirection.None;
    }
}