using UnityEngine;

public class MenuScroller : MonoBehaviour {
    
    private RectTransform mainScreen;
    private RectTransform levelScroller;
    private KeyPointsHandler keyPoints;
    
    void Awake()
    {
        mainScreen = GameObject.Find("MainScreen").GetComponent<RectTransform>();
        levelScroller = GameObject.Find("LevelScrollRect").GetComponent<RectTransform>();
        keyPoints = GameObject.FindGameObjectWithTag("Scripts").GetComponent<KeyPointsHandler>();

        levelScroller.position = keyPoints.LevelMapStart.transform.position;
    }

	private void FixedUpdate ()
    {
        float xDiff = keyPoints.MainMenuCamPoint.transform.position.x - mainScreen.position.x;
        mainScreen.position += Vector3.right * xDiff;
        levelScroller.position += Vector3.right * xDiff;
	}
}
