using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPositioner : MonoBehaviour {

    public ClumsyMainMenu Clumsy;

    private Camera mainCam;
    private CameraFollowObject camFollow;
    private KeyPointsHandler keyPoints;
    private Vector2 targetPosition;

    private const float maxCamDistPerFrame = 2f;

    private enum CamStates
    {
        Idle, Moving
    }
    private CamStates state;

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
    
    private void Awake()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        keyPoints = GameObject.FindGameObjectWithTag("Scripts").GetComponent<KeyPointsHandler>();
        camFollow = mainCam.GetComponent<CameraFollowObject>();
    }

	private void Start ()
    {
        SetCamPositionFromPointImmediate(keyPoints.MainMenuCamPoint);
        StartCoroutine(MainMenu());
    }
	
	private void Update ()
    {
        float xDist = Vector3.Lerp(mainCam.transform.position, targetPosition, Time.deltaTime * 4).x - mainCam.transform.position.x;
        xDist = Mathf.Clamp(xDist, -maxCamDistPerFrame, maxCamDistPerFrame);
        mainCam.transform.position += Vector3.right * xDist;
    }
    

    private IEnumerator MainMenu()
    {
        state = CamStates.Moving;
        SetTargetPosition(keyPoints.MainMenuCamPoint);

        while (Mathf.Abs(mainCam.transform.position.x - targetPosition.x) > 0.1f)
        {
            yield return null;
        }
        state = CamStates.Idle;

        Clumsy.SetPosition(keyPoints.EntryPoint.transform.position);
        Clumsy.MoveToPoint(keyPoints.EntryLandingPoint.transform.position);
    }

    private IEnumerator LevelMenu()
    {
        state = CamStates.Moving;
        Clumsy.MoveToPoint(Vector3.zero);
        while (Vector2.Distance(Clumsy.transform.position, Vector2.zero) > 0.4f)
        {
            yield return null;
        }
        Clumsy.Dash();

        float timer = 0f;
        float duration = 0.5f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            SetCamPositionFromPointImmediate(new Vector3(mainCam.transform.position.x + Time.deltaTime * (timer * 10f + 3f), 0f, 0f));
            yield return null;
        }
        SetTargetPosition(keyPoints.LevelMenuMidPoint);

        Clumsy.SetPosition(keyPoints.LevelEntryPoint.transform.position);
        Clumsy.Dash();
        yield return new WaitForSeconds(.5f);
        Clumsy.MoveToPoint(keyPoints.LevelMenuEndPoint.transform.position);
        while (!Clumsy.TargetReached())
        {
            Clumsy.RemainUnperched();
            yield return null;
        }
        while (!Clumsy.IsPerched())
        {
            yield return null;
        }
        SetTargetPosition(keyPoints.LevelMapStart);
        state = CamStates.Idle;
    }

    private void SetCamPositionFromPointImmediate(GameObject objToMoveTo)
    {
        mainCam.transform.position = GetPosFromPoint(objToMoveTo);
    }
    private void SetCamPositionFromPointImmediate(Vector3 pt)
    {
        targetPosition = pt;
        mainCam.transform.position = new Vector3(pt.x, pt.y, mainCam.transform.position.z);
    }

    private void SetTargetPosition(GameObject targetObj)
    {
        targetPosition = GetPosFromPoint(targetObj);
    }

    private IEnumerator MoveCameraPoint(GameObject objToMoveTo)
    {
        Vector3 targetPos = GetPosFromPoint(objToMoveTo);
        while (Vector3.Distance(mainCam.transform.position, targetPos) > 0.05f)
        {
            float xDist = Vector3.Lerp(mainCam.transform.position, targetPos, Time.deltaTime * 4).x - mainCam.transform.position.x;
            xDist = Mathf.Clamp(xDist, -1f, 1f);
            mainCam.transform.position += Vector3.right * xDist;
            yield return null;
        }
    }

    private Vector3 GetPosFromPoint(GameObject pointObj)
    {

        float xPos = pointObj.transform.position.x;
        float yPos = pointObj.transform.position.y;
        float zPos = mainCam.transform.position.z;
        return new Vector3(xPos, yPos, zPos);
    }
}
