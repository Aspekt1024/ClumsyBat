using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AirFollowAction : BaseAction {

    public float FollowSpeed;

    public enum Ifaces
    {
        Input, Direction,
        Next
    }

    private const float maxDegreesPerSecond = 120f;
        
    private GameObject followingObject;
    private Rigidbody2D bossBody;
    private SpriteRenderer bossRenderer;
    
    public override void ActivateBehaviour()
    {
        IsActive = false;
        Follow();
        CallNext((int)Ifaces.Next);
    }

    private void Follow()
    {
        bool isFlipped = bossRenderer.flipX;
        float rotation = GetAdditionalRotation(Time.deltaTime);
        bossBody.rotation += rotation;

        if (Mathf.Abs(bossBody.rotation) > 95f)
        {
            bossBody.rotation -= Mathf.Clamp(bossBody.rotation, -1, 1) * 180f;
            bossRenderer.flipX = !isFlipped;
        }

        bossBody.velocity = new Vector2(bossBody.transform.right.x, bossBody.transform.right.y) * FollowSpeed * (bossBody.GetComponent<SpriteRenderer>().flipX ? 1 : -1);
    }
    
    public override void GameSetup(BehaviourSet owningContainer, BossData behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);

        ActionConnection conn = GetInterface((int)Ifaces.Direction);
        followingObject = conn.ConnectedInterface.Action.GetObject(conn.OtherConnID);

        bossBody = boss.GetComponent<Boss>().Body;
        bossRenderer = bossBody.GetComponent<SpriteRenderer>();
    }

    public override void Stop()
    {
        IsActive = false;
    }

    private float GetAdditionalRotation(float deltaTime)
    {
        Vector2 dist = followingObject.transform.position - boss.transform.position;

        float radToDeg = 57.295779513f;
        float targetRotation = Mathf.Atan(Mathf.Abs(dist.y / dist.x)) * radToDeg;

        if (dist.x < 0 && dist.y > 0) targetRotation = -targetRotation;
        else if (dist.x > 0 && dist.y > 0) targetRotation = targetRotation - 180;
        else if (dist.x > 0 && dist.y < 0) targetRotation = 180 - targetRotation;

        if (bossRenderer.flipX)
            targetRotation -= 180;

        float requiredRotation = targetRotation - boss.transform.eulerAngles.z;
        
        while (requiredRotation > 180) requiredRotation -= 360;
        while (requiredRotation < -180) requiredRotation += 360;
        
        return Mathf.Clamp(requiredRotation, -maxDegreesPerSecond * deltaTime, maxDegreesPerSecond * deltaTime);
    }
}
