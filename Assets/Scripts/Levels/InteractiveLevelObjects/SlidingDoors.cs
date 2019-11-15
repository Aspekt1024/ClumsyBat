using UnityEngine;
using System.Collections;
using ClumsyBat;
using UnityEngine.Analytics;

public class SlidingDoors : MonoBehaviour {

#pragma warning disable 649
    [SerializeField] private Transform topDoor;
    [SerializeField] private Transform bottomDoor;
#pragma warning restore 649
    
    private const float TopOpenY = 9f;
    private const float BottomOpenY = -8f;
    private const float TopClosedY =  2.11f;
    private const float BottomClosedY = -2.97f;

    public void Close()
    {
        StartCoroutine(CloseRoutine());
    }

    public void OpenImmediate()
    {
        topDoor.position = new Vector2(topDoor.position.x, TopOpenY);
        bottomDoor.position = new Vector2(bottomDoor.position.x, BottomOpenY);
    }

    private IEnumerator CloseRoutine()
    {
        const float slideDuration = 0.6f;
        float slideTimer = 0f;

        while (slideTimer < slideDuration)
        {
            slideTimer += Time.deltaTime;
            float ratio = Mathf.Clamp(slideTimer / slideDuration, 0f, 1f);
            float topPosY = TopOpenY - (TopOpenY - TopClosedY) * Mathf.Pow(ratio, 7);
            float bottomPosY = BottomOpenY - (BottomOpenY - BottomClosedY) * Mathf.Pow(ratio, 7);
            topDoor.position = new Vector2(topDoor.position.x, topPosY);
            bottomDoor.position = new Vector2(bottomDoor.position.x, bottomPosY);
            yield return null;
        }

        GameStatics.Camera.Shake(0.5f);
    }

    private IEnumerator OpenRoutine()
    {
        const float slideDuration = 0.4f;
        float slideTimer = 0f;

        while (slideTimer < slideDuration)
        {
            slideTimer += Time.deltaTime;
            float ratio = slideTimer / slideDuration;
            float topPosY = TopClosedY - (TopClosedY - TopOpenY) * ratio;
            float bottomPosY = BottomClosedY - (BottomClosedY - BottomOpenY) * ratio;
            topDoor.position = new Vector2(topDoor.position.x, topPosY);
            bottomDoor.position = new Vector2(bottomDoor.position.x, bottomPosY);
            yield return null;
        }
    }
}
