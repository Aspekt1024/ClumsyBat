using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPositioner : MonoBehaviour {

    public ClumsyMainMenu Clumsy;

    private Camera mainCam;
    private KeyPointsHandler keyPoints;

    public void MoveToLevelMenu()
    {
        StartCoroutine(LevelMenu());
    }
    
    private void Awake()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        keyPoints = GameObject.FindGameObjectWithTag("Scripts").GetComponent<KeyPointsHandler>();
    }

	private void Start ()
    {
        SetCamPositionFromPointImmediate(keyPoints.MainMenuCamPoint);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator LevelMenu()
    {
        float timer = 0f;
        float duration = 0.5f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            SetCamPositionFromPointImmediate(new Vector3(mainCam.transform.position.x + Time.deltaTime * (timer * 10f + 3f), 0f, 0f));
            yield return null;
        }
        yield return StartCoroutine(MoveCameraPoint(keyPoints.LevelCamPoint));
        Clumsy.SetPosition(keyPoints.LevelEntryPoint.transform.position);
        Clumsy.Dash();
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(MoveCameraPoint(keyPoints.LevelMapStart));
    }

    private void SetCamPositionFromPointImmediate(GameObject objToMoveTo)
    {
        mainCam.transform.position = GetPosFromPoint(objToMoveTo);
    }
    private void SetCamPositionFromPointImmediate(Vector3 pt)
    {
        mainCam.transform.position = new Vector3(pt.x, pt.y, mainCam.transform.position.z);
    }

    private IEnumerator MoveCameraPoint(GameObject objToMoveTo)
    {
        Vector3 targetPos = GetPosFromPoint(objToMoveTo);
        while (Vector3.Distance(mainCam.transform.position, targetPos) > 0.05f)
        {
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPos, Time.deltaTime * 4f);
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
