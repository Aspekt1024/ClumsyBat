using UnityEngine;
using ClumsyBat;

public class CameraFollowObject : MonoBehaviour {

    public Transform ObjectToFollow;

    public const float BASE_FOLLOW_SPEED = 4f;

    public float XOffset = 4f; // This is for clumsy, but for the menu we need to set to 0... 

    private bool following = false;
    private float endPointX;
    private bool stopFollowingAtEndpoint;
    private float followSpeed = BASE_FOLLOW_SPEED;
    
    public void SetEndPoint(float endPoint)
    {
        endPointX = endPoint;
    }

    public void StartFollowing(Transform target = null, float followSpeed = BASE_FOLLOW_SPEED)
    {
        following = true;
        this.followSpeed = followSpeed;

        if (target != null)
        {
            ObjectToFollow = target;
        }
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

        if (ObjectToFollow == null) return;

        if (!following || (GameStatics.GameManager.IsInLevel && ObjectToFollow.position.x + XOffset < 0) || transform.position.x > endPointX) return;

        float xPos = Mathf.Lerp(transform.position.x, ObjectToFollow.position.x + XOffset, Time.deltaTime * followSpeed);
        Vector3 pos = transform.position;
        pos.x = xPos;
        transform.position = pos;
	}
}
