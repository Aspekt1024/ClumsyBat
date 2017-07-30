using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPiece : MonoBehaviour {

    public Rigidbody2D OtherBody;
    public Vector2 PointOnOther;
    public bool CenterPoints;
    public Vector2 PointOnThis;
    public bool LeadingPiece;

    private float desiredDistance = 0.05f;
    private float maxDegreesPerSecond = 1800f;
    private float followSpeed = 5f;

    private Rigidbody2D thisBody;
    private bool isAttached;

    private float oscTimer;
    private bool oscillatingUp;
    private const float oscDuration = 1.6f;
    private float oscRotation;
    
    private bool hasDest;

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
        
        FollowOther();
    }

    private void FollowOther()
    {
        bool isFlipped = thisBody.GetComponent<SpriteRenderer>().flipX;
        Vector2 thisPointInWorldSpace = thisBody.position + V3ToV2((isFlipped ? -1 : 1) * thisBody.transform.right * PointOnThis.x + thisBody.transform.up * PointOnThis.y);

        if (OtherBody.GetComponent<SpriteRenderer>().flipX != isFlipped)
        {
            isFlipped = OtherBody.GetComponent<SpriteRenderer>().flipX;
            // TODO flip this properly, don't make pieces jump, it looks really bad.
            thisBody.GetComponent<SpriteRenderer>().flipX = isFlipped;
            PointOnOther = new Vector2(-PointOnOther.x, PointOnOther.y);
        }
        
        Vector2 otherPointInWorldSpace = OtherBody.position + V3ToV2(OtherBody.transform.right * PointOnOther.x + OtherBody.transform.up * PointOnOther.y);

        if (Vector2.Distance(thisPointInWorldSpace, otherPointInWorldSpace) > desiredDistance)
        {
            thisBody.velocity = Vector2.zero;
            transform.Rotate(Vector3.forward * GetAdditionalRotation(Time.deltaTime, otherPointInWorldSpace));
            thisPointInWorldSpace = thisBody.position + V3ToV2((isFlipped ? -1 : 1) * thisBody.transform.right * PointOnThis.x + thisBody.transform.up * PointOnThis.y);
            Vector2 distToAdd = Vector2.Lerp(thisPointInWorldSpace, otherPointInWorldSpace, Vector2.Distance(thisPointInWorldSpace, otherPointInWorldSpace) / (Vector2.Distance(thisPointInWorldSpace, otherPointInWorldSpace) - desiredDistance)) - thisPointInWorldSpace;
            thisBody.position += distToAdd;
        }
        else
        {
            // TODO get angle and position to settle properly
            transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(transform.eulerAngles.z, OtherBody.transform.eulerAngles.z, Time.deltaTime / 10f));
            thisPointInWorldSpace = thisBody.position + V3ToV2((isFlipped ? -1 : 1) * thisBody.transform.right * PointOnThis.x + thisBody.transform.up * PointOnThis.y);
            
            Vector2 distToAdd = Vector2.Lerp(thisPointInWorldSpace, otherPointInWorldSpace, Time.deltaTime * followSpeed) - thisPointInWorldSpace;
            thisBody.position += distToAdd;
        }
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
            //Detach();
        }
    }

    private Vector2 V3ToV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }

    private float GetAdditionalRotation(float deltaTime, Vector2 dest)
    {
        Vector2 dist = dest - V3ToV2(transform.position);

        float radToDeg = 57.295779513f;
        float targetRotation = Mathf.Atan(Mathf.Abs(dist.y / dist.x)) * radToDeg;

        if (dist.x < 0 && dist.y > 0) targetRotation = -targetRotation;
        else if (dist.x > 0 && dist.y > 0) targetRotation = targetRotation - 180;
        else if (dist.x > 0 && dist.y < 0) targetRotation = 180 - targetRotation;

        if (GetComponent<SpriteRenderer>().flipX)
            targetRotation -= 180;

        float requiredRotation = targetRotation - transform.eulerAngles.z;

        while (requiredRotation > 180) requiredRotation -= 360;
        while (requiredRotation < -180) requiredRotation += 360;

        return Mathf.Clamp(requiredRotation, -maxDegreesPerSecond * deltaTime, maxDegreesPerSecond * deltaTime);
    }
}
