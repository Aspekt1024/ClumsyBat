using ClumsyBat;
using ClumsyBat.Players;
using System.Collections;
using UnityEngine;

public class CamPositioner : MonoBehaviour {

    public Player Clumsy;
    //public NavButtonHandler NavButtons;
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

    private bool targetReached;

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
        Debug.Log("still in use");
        keyPoints = GameObject.FindGameObjectWithTag("Scripts").GetComponent<KeyPointsHandler>();
        camFollow = FindObjectOfType<CameraFollowObject>();
        camFollow.StopFollowing();
    }

	private void Start ()
    {
        Clumsy = GameStatics.Player.Clumsy;
        mainScreen = GameObject.Find("MainScreen").GetComponent<RectTransform>();
        levelScroller = GameObject.Find("LevelScrollRect").GetComponent<RectTransform>();
        Vector3 levelScrollPos = keyPoints.LevelMapStart.transform.position;
        levelScroller.position = new Vector3(levelScrollPos.x, GameStatics.Camera.MenuCamera.transform.position.y, levelScroller.position.z);

        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            //NavButtons.SetNavButtons(Positions.LevelSelect);
            SetCamPositionFromPointImmediate(keyPoints.LevelMapStart);
            LevelButtons.SetCurrentLevel(GameStatics.LevelManager.Level);
        }
        else
        {
            SetCamPositionFromPointImmediate(keyPoints.MainMenuCamPoint);
            StartCoroutine(MainMenu());
        }
        GameStatics.UI.LoadingScreen.HideLoadScreen(0.4f);
    }
	
	private void FixedUpdate ()
    {
        float xDist = Mathf.Lerp(GameStatics.Camera.MenuCamera.transform.position.x, targetPosition.x, Time.fixedDeltaTime * 4) - GameStatics.Camera.MenuCamera.transform.position.x;
        xDist = Mathf.Clamp(xDist, -maxCamDistPerFrame, maxCamDistPerFrame);
        GameStatics.Camera.MenuCamera.transform.position += Vector3.right * xDist;

        float xDiff = keyPoints.MainMenuCamPoint.transform.position.x - mainScreen.position.x;
        mainScreen.position += Vector3.right * xDiff;
        levelScroller.position += Vector3.right * xDiff;
    }
    

    private IEnumerator MainMenu()
    {
        if (Mathf.Abs(Clumsy.Model.position.x - keyPoints.EntryLandingPoint.transform.position.x) > 1f)
        {
            Clumsy.Model.position = keyPoints.EntryPoint.transform.position;
        }

        state = CamStates.Moving;
        //NavButtons.DisableNavButtons();
        yield return StartCoroutine(LevelButtons.MoveLevelMapToStart());
        SetTargetPosition(keyPoints.MainMenuCamPoint);
        
        while (Mathf.Abs(GameStatics.Camera.MenuCamera.transform.position.x - targetPosition.x) > 0.1f)
        {
            yield return null;
        }
        state = CamStates.Idle;

        if (Mathf.Abs(Clumsy.Model.position.x -keyPoints.EntryLandingPoint.transform.position.x) > 1f)
        {
            Clumsy.Model.position = keyPoints.EntryPoint.transform.position;
            GameStatics.Player.PossessByAI();
            GameStatics.Player.AIController.MoveTo(keyPoints.EntryLandingPoint.transform.position);
        }
        //NavButtons.SetNavButtons(Positions.Main);
    }

    private IEnumerator DropdownArea()
    {
        //NavButtons.DisableNavButtons();
        state = CamStates.Moving;

        SetTargetPosition(keyPoints.DropdownAreaPoint);

        while (Mathf.Abs(GameStatics.Camera.MenuCamera.transform.position.x - targetPosition.x) > 0.1f)
        {
            yield return null;
        }
        state = CamStates.Idle;
        //NavButtons.SetNavButtons(Positions.DropdownArea);
    }

    private IEnumerator LevelMenu()
    {
        //NavButtons.DisableNavButtons();
        state = CamStates.Moving;

        GameStatics.Player.PossessByAI();
        var aiController = GameStatics.Player.AIController;

        aiController.MoveTo(new Vector3(0, GameStatics.Camera.MenuCamera.transform.position.y, 0), TargetReached);
        targetReached = false;
        Clumsy.Abilities.Perch.Disable();

        while (!targetReached)
        {
            yield return null;
        }

        Clumsy.DoAction(ClumsyAbilityHandler.DirectionalActions.Dash, MovementDirections.Right);

        float timer = 0f;
        float duration = 0.5f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            SetCamPositionFromPointImmediate(new Vector3(GameStatics.Camera.MenuCamera.transform.position.x + Time.deltaTime * (timer * 10f + 3f), GameStatics.Camera.MenuCamera.transform.position.y, 0f));
            yield return null;
        }
        SetTargetPosition(keyPoints.LevelMenuMidPoint);

        Clumsy.Model.position = keyPoints.LevelEntryPoint.transform.position;
        Clumsy.DoAction(ClumsyAbilityHandler.DirectionalActions.Dash, MovementDirections.Right);
        yield return new WaitForSeconds(.5f);

        aiController.MoveTo(keyPoints.LevelMenuEndPoint.transform.position);
        camFollow.SetEndPoint(keyPoints.LevelMapStart.transform.position.x);
        camFollow.StartFollowing();

        targetReached = false;
        while (targetReached)
        {
            yield return null;
        }
        Clumsy.Abilities.Perch.Enable();
        while (!Clumsy.State.IsPerched)
        {
            yield return null;
        }

        camFollow.StopFollowing();
        SetTargetPosition(keyPoints.LevelMapStart);
        state = CamStates.Idle;
        //NavButtons.SetNavButtons(Positions.LevelSelect);
    }

    private void TargetReached()
    {
        targetReached = true;
    }

    private void SetCamPositionFromPointImmediate(GameObject objToMoveTo)
    {
        SetTargetPosition(objToMoveTo);
        GameStatics.Camera.MenuCamera.transform.position = GetPosFromPoint(objToMoveTo);
    }
    private void SetCamPositionFromPointImmediate(Vector3 pt)
    {
        targetPosition = pt;
        GameStatics.Camera.MenuCamera.transform.position = new Vector3(pt.x, pt.y, GameStatics.Camera.MenuCamera.transform.position.z);
    }

    private void SetTargetPosition(GameObject targetObj)
    {
        targetPosition = GetPosFromPoint(targetObj);
    }

    private IEnumerator MoveCameraPoint(GameObject objToMoveTo)
    {
        Vector3 targetPos = GetPosFromPoint(objToMoveTo);
        while (Vector3.Distance(GameStatics.Camera.MenuCamera.transform.position, targetPos) > 0.05f)
        {
            float xDist = Vector3.Lerp(GameStatics.Camera.MenuCamera.transform.position, targetPos, Time.deltaTime * 4).x - GameStatics.Camera.MenuCamera.transform.position.x;
            xDist = Mathf.Clamp(xDist, -1f, 1f);
            GameStatics.Camera.MenuCamera.transform.position += Vector3.right * xDist;
            yield return null;
        }
    }

    private Vector3 GetPosFromPoint(GameObject pointObj)
    {
        float xPos = pointObj.transform.position.x;
        float yPos = pointObj.transform.position.y;
        float zPos = GameStatics.Camera.MenuCamera.transform.position.z;
        return new Vector3(xPos, yPos, zPos);
    }
}
