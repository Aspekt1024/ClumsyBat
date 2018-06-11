using ClumsyBat.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPositioner : MonoBehaviour {

    public ClumsyMainMenu Clumsy;
    public NavButtonHandler NavButtons;
    public LevelButtonHandler LevelButtons;

    public enum Positions
    {
        Main, DropdownArea, LevelSelect
    }
    
    private CameraFollowObject camFollow;
    private KeyPointsHandler keyPoints;
    private Vector2 targetPosition;

    private RectTransform mainScreen;
    private RectTransform levelScroller;

    private const float maxCamDistPerFrame = 2f;

    private enum CamStates
    {
        Idle, Moving
    }
    private CamStates state;

    public bool IsMoving()
    {
        return state == CamStates.Moving;
    }

    public void MoveToLevelMenu()
    {
        if (state == CamStates.Moving) return;
        StartCoroutine(LevelMenu());
    }
    
    public void MoveToMainMenu()
    {
        if (state == CamStates.Moving) return;
        StartCoroutine(MainMenu());
    }

    public void MoveToDropdownArea()
    {
        if (state == CamStates.Moving) return;
        StartCoroutine(DropdownArea());
    }
    
    private void Awake()
    {
        keyPoints = GameObject.FindGameObjectWithTag("Scripts").GetComponent<KeyPointsHandler>();
        camFollow = CameraManager.Instance.MenuCamera.GetComponent<CameraFollowObject>();
        camFollow.StopFollowing();
    }

	private void Start ()
    {
        mainScreen = GameObject.Find("MainScreen").GetComponent<RectTransform>();
        levelScroller = GameObject.Find("LevelScrollRect").GetComponent<RectTransform>();
        Vector3 levelScrollPos = keyPoints.LevelMapStart.transform.position;
        levelScroller.position = new Vector3(levelScrollPos.x, CameraManager.Instance.MenuCamera.transform.position.y, levelScroller.position.z);

        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            NavButtons.SetNavButtons(Positions.LevelSelect);
            SetCamPositionFromPointImmediate(keyPoints.LevelMapStart);
            LevelButtons.SetCurrentLevel((int)GameData.Instance.Level);
        }
        else
        {
            SetCamPositionFromPointImmediate(keyPoints.MainMenuCamPoint);
            StartCoroutine(MainMenu());
        }
        OverlayManager.Instance.LoadScreen.HideLoadScreen(0.4f);
    }
	
	private void FixedUpdate ()
    {
        float xDist = Mathf.Lerp(CameraManager.Instance.MenuCamera.transform.position.x, targetPosition.x, Time.deltaTime * 4) - CameraManager.Instance.MenuCamera.transform.position.x;
        xDist = Mathf.Clamp(xDist, -maxCamDistPerFrame, maxCamDistPerFrame);
        CameraManager.Instance.MenuCamera.transform.position += Vector3.right * xDist;

        float xDiff = keyPoints.MainMenuCamPoint.transform.position.x - mainScreen.position.x;
        mainScreen.position += Vector3.right * xDiff;
        levelScroller.position += Vector3.right * xDiff;
    }
    

    private IEnumerator MainMenu()
    {
        if (Mathf.Abs(Clumsy.transform.position.x - keyPoints.EntryLandingPoint.transform.position.x) > 1f)
        {
            Clumsy.SetPosition(keyPoints.EntryPoint.transform.position);
        }

        state = CamStates.Moving;
        NavButtons.DisableNavButtons();
        yield return StartCoroutine(LevelButtons.MoveLevelMapToStart());
        SetTargetPosition(keyPoints.MainMenuCamPoint);
        
        while (Mathf.Abs(CameraManager.Instance.MenuCamera.transform.position.x - targetPosition.x) > 0.1f)
        {
            yield return null;
        }
        state = CamStates.Idle;

        if (Mathf.Abs(Clumsy.transform.position.x -keyPoints.EntryLandingPoint.transform.position.x) > 1f)
        {
            Clumsy.SetPosition(keyPoints.EntryPoint.transform.position);
            Clumsy.MoveToPoint(keyPoints.EntryLandingPoint.transform.position);
        }
        NavButtons.SetNavButtons(Positions.Main);
    }

    private IEnumerator DropdownArea()
    {
        NavButtons.DisableNavButtons();
        state = CamStates.Moving;

        SetTargetPosition(keyPoints.DropdownAreaPoint);

        while (Mathf.Abs(CameraManager.Instance.MenuCamera.transform.position.x - targetPosition.x) > 0.1f)
        {
            yield return null;
        }
        state = CamStates.Idle;
        NavButtons.SetNavButtons(Positions.DropdownArea);
    }

    private IEnumerator LevelMenu()
    {
        NavButtons.DisableNavButtons();
        state = CamStates.Moving;

        Clumsy.MoveToPoint(new Vector3(0, CameraManager.Instance.MenuCamera.transform.position.y, 0));
        while (!Clumsy.TargetReached())
        {
            Clumsy.RemainUnperched();
            yield return null;
        }
        Clumsy.Dash();

        float timer = 0f;
        float duration = 0.5f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            SetCamPositionFromPointImmediate(new Vector3(CameraManager.Instance.MenuCamera.transform.position.x + Time.deltaTime * (timer * 10f + 3f), CameraManager.Instance.MenuCamera.transform.position.y, 0f));
            yield return null;
        }
        SetTargetPosition(keyPoints.LevelMenuMidPoint);

        Clumsy.SetPosition(keyPoints.LevelEntryPoint.transform.position);
        Clumsy.Dash();
        yield return new WaitForSeconds(.5f);

        Clumsy.MoveToPoint(keyPoints.LevelMenuEndPoint.transform.position);
        camFollow.SetEndPoint(keyPoints.LevelMapStart.transform.position.x);
        camFollow.StartFollowing();

        while (!Clumsy.TargetReached())
        {
            Clumsy.RemainUnperched();
            yield return null;
        }
        while (!Clumsy.IsPerched())
        {
            yield return null;
        }

        camFollow.StopFollowing();
        SetTargetPosition(keyPoints.LevelMapStart);
        state = CamStates.Idle;
        NavButtons.SetNavButtons(Positions.LevelSelect);
    }

    private void SetCamPositionFromPointImmediate(GameObject objToMoveTo)
    {
        SetTargetPosition(objToMoveTo);
        CameraManager.Instance.MenuCamera.transform.position = GetPosFromPoint(objToMoveTo);
    }
    private void SetCamPositionFromPointImmediate(Vector3 pt)
    {
        targetPosition = pt;
        CameraManager.Instance.MenuCamera.transform.position = new Vector3(pt.x, pt.y, CameraManager.Instance.MenuCamera.transform.position.z);
    }

    private void SetTargetPosition(GameObject targetObj)
    {
        targetPosition = GetPosFromPoint(targetObj);
    }

    private IEnumerator MoveCameraPoint(GameObject objToMoveTo)
    {
        Vector3 targetPos = GetPosFromPoint(objToMoveTo);
        while (Vector3.Distance(CameraManager.Instance.MenuCamera.transform.position, targetPos) > 0.05f)
        {
            float xDist = Vector3.Lerp(CameraManager.Instance.MenuCamera.transform.position, targetPos, Time.deltaTime * 4).x - CameraManager.Instance.MenuCamera.transform.position.x;
            xDist = Mathf.Clamp(xDist, -1f, 1f);
            CameraManager.Instance.MenuCamera.transform.position += Vector3.right * xDist;
            yield return null;
        }
    }

    private Vector3 GetPosFromPoint(GameObject pointObj)
    {
        float xPos = pointObj.transform.position.x;
        float yPos = pointObj.transform.position.y;
        float zPos = CameraManager.Instance.MenuCamera.transform.position.z;
        return new Vector3(xPos, yPos, zPos);
    }
}
