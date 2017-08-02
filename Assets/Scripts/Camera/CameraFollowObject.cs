using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour {

    public Transform ObjectToFollow;

    private bool following = true;
    private const float xOffset = 4f;
    private float endPointX;
    private bool stopFollowingAtEndpoint;
    
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

    public void StopFollowingAtEndPoint()
    {
        stopFollowingAtEndpoint = true;
    }
    
    private void FixedUpdate()
    {
        if (transform.position.x > endPointX && stopFollowingAtEndpoint)
        {
            following = false;
            return;
        }

        if (!following || ObjectToFollow.position.x + xOffset < 0 || transform.position.x > endPointX) return;
        
        float xPos = Mathf.Lerp(transform.position.x, ObjectToFollow.position.x + xOffset, Time.deltaTime * 4f);
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
	}
}
