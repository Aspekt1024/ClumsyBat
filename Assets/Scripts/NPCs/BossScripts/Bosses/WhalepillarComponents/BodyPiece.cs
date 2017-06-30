using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPiece : MonoBehaviour {

    public Rigidbody2D OtherBody;
    public Vector2 PointOnOther;
    public bool CenterPoints;
    public Vector2 PointOnThis;
    public bool LeadingPiece;

    private float desiredDistance = 5f;
    private float maxDegreesPerSecond = 180f;

    private Rigidbody2D thisBody;
    private bool isAttached;

    private float oscTimer;
    private bool oscillatingUp;
    private const float oscDuration = 1.6f;
    private float oscRotation;
    
    public void Detach()
    {
        isAttached = false;
        thisBody.gravityScale = 1;
    }

    private void Start ()
    {
        thisBody = GetComponent<Rigidbody2D>();
        isAttached = true;
        oscRotation = 7;
	}

    private void FixedUpdate()
    {
        if (OtherBody == null || !isAttached || Toolbox.Instance.GamePaused) return;
        
        MovePointsTogether();
    }
    
    private void MovePointsTogether()
    {
        Vector2 otherPointInWorldSpace = OtherBody.position + V3ToV2(OtherBody.transform.right * PointOnOther.x + OtherBody.transform.up * PointOnOther.y);
        Vector2 distCentreToPoint = otherPointInWorldSpace - V3ToV2(transform.position);

        Vector2 thisPointInWorldSpace = thisBody.position + V3ToV2(thisBody.transform.right * PointOnThis.x + thisBody.transform.up * PointOnThis.y);
        Vector2 pointsDist = otherPointInWorldSpace - thisPointInWorldSpace;

        if (pointsDist.magnitude < desiredDistance)
        {

            if (pointsDist.magnitude < 0.05f)
            {
                    OscillateRotation();
            }
            else
            {
                float additionalRotation = Mathf.Clamp(OtherBody.rotation - thisBody.rotation, -360 * Time.deltaTime, 360 * Time.deltaTime);
                transform.Rotate(Vector3.forward, additionalRotation);
            }

        }
        else
        {
            float radToDeg = 57.295779513f;
            float targetRotation = Mathf.Atan(distCentreToPoint.y / distCentreToPoint.x) * radToDeg;

            if (distCentreToPoint.x < 0)
                targetRotation = 180 + targetRotation;
            else if (distCentreToPoint.y < 0)
                targetRotation = 360 + targetRotation;

            targetRotation += 180;

            float requiredRotation = targetRotation - thisBody.rotation;

            if (requiredRotation > 180)
                requiredRotation -= 360f;

            float additionalRotation = Mathf.Clamp(requiredRotation, -maxDegreesPerSecond * Time.deltaTime, maxDegreesPerSecond * Time.deltaTime);
            thisBody.rotation += additionalRotation;
        }

        thisPointInWorldSpace = thisBody.position + V3ToV2(thisBody.transform.right * PointOnThis.x + thisBody.transform.up * PointOnThis.y);
        pointsDist = otherPointInWorldSpace - thisPointInWorldSpace;
        thisBody.velocity = pointsDist * 14;
        
    }

    private void OscillateRotation()
    {
        oscTimer += Time.deltaTime;

        if (oscTimer > oscDuration)
        {
            oscillatingUp = !oscillatingUp;
            oscTimer = 0;
        }

        oscRotation += 8 * Time.deltaTime * (oscillatingUp ? 1 : -1);
        thisBody.rotation = oscRotation;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Player")
        {
            Detach();
        }
    }

    private Vector2 V3ToV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
}
