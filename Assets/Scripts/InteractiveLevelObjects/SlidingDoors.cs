using UnityEngine;
using System.Collections;

public class SlidingDoors : MonoBehaviour {

    private Transform _topDoor;
    private Transform _bottomDoor;

	private void Start ()
    {
        GetDoorComponents();
	}
	
	private void Update ()
    {
		
	}

    public void Close()
    {
        StartCoroutine("SlideDown");
    }

    private void GetDoorComponents()
    {
        foreach (Transform tf in gameObject.transform)
        {
            if (tf.name == "TopDoor")
                _topDoor = tf;
            if (tf.name == "BottomDoor")
                _bottomDoor = tf;
        }
    }

    // TODO add shake

    private IEnumerator SlideDown()
    {
        const float slideDuration = 0.6f;
        float slideTimer = 0f;

        const float topEndY = 2.11f;
        const float bottomEndY = -2.97f;
        float topStartY = _topDoor.position.y;
        float bottomStartY = _bottomDoor.position.y;


        // TODO add pause
        while (slideTimer < slideDuration)
        {
            slideTimer += Time.deltaTime;
            float ratio = Mathf.Clamp(slideTimer / slideDuration, 0f, 1f);
            float topPosY = topStartY - (topStartY - topEndY) * Mathf.Pow(ratio, 7);
            float bottomPosY = bottomStartY - (bottomStartY - bottomEndY) * Mathf.Pow(ratio, 7);
            _topDoor.position = new Vector2(_topDoor.position.x, topPosY);
            _bottomDoor.position = new Vector2(_bottomDoor.position.x, bottomPosY);
            yield return null;
        }

        CameraEventListener.CameraShake();
        yield return new WaitForSeconds(0.5f);
        CameraEventListener.StopCameraShake();
    }

    private IEnumerator SlideUp()
    {
        const float slideDuration = 0.4f;
        float slideTimer = 0f;

        const float topEndY = 9f;
        const float bottomEndY = -8f;
        float topStartY = _topDoor.position.y;
        float bottomStartY = _bottomDoor.position.y;

        while (slideTimer < slideDuration)
        {
            slideTimer += Time.deltaTime;
            float ratio = slideTimer / slideDuration;
            float topPosY = topStartY - (topStartY - topEndY) * ratio;
            float bottomPosY = bottomStartY - (bottomStartY - bottomEndY) * ratio;
            _topDoor.position = new Vector2(_topDoor.position.x, topPosY);
            _bottomDoor.position = new Vector2(_bottomDoor.position.x, bottomPosY);
            yield return null;
        }
    }
}
