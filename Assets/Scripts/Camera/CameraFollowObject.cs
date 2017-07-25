using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour {

    public Transform ObjectToFollow;

    private bool following = true;
    private const float xOffset = 4f;
    private float endPointX;
    
    public void SetEndPoint(float endPoint)
    {
        endPointX = endPoint;
    }

    public void StartFollowing()
    {
        following = true;
    }

    public void StopFollowing()
    {
        following = false;
    }

	private void FixedUpdate()
    {
        if (!following || ObjectToFollow.position.x + xOffset < 0 || transform.position.x > endPointX) return;
        
        float xPos = Mathf.Lerp(transform.position.x, ObjectToFollow.position.x + xOffset, Time.deltaTime * 4f);
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
	}
}
